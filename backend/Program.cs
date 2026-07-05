using DocumentsApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.MapGet("/api/documents", async (AppDbContext db, [AsParameters] DocumentFilter filter) =>
{
    var query = db.Documents.AsQueryable();

    if (!string.IsNullOrWhiteSpace(filter.Title))
        query = query.Where(d =>
            d.Title.ToLower().Contains(filter.Title.ToLower()));

    if (!string.IsNullOrWhiteSpace(filter.Description))
        query = query.Where(d =>
            d.Description.ToLower().Contains(filter.Description.ToLower()));

    if (!string.IsNullOrWhiteSpace(filter.ResponsibleUnit))
        query = query.Where(d => d.ResponsibleUnit == filter.ResponsibleUnit);

    if (filter.CreatedAt != null)
        query = query.Where(d => d.CreatedAt == filter.CreatedAt);

    if (!string.IsNullOrWhiteSpace(filter.Url))
        query = query.Where(d => d.Url.ToLower().Contains(filter.Url.ToLower()));

    if (!string.IsNullOrWhiteSpace(filter.FileType))
        query = query.Where(d => d.FileType == filter.FileType);

    if (filter.EstimatedReadingTimeMinutes != null)
        query = query.Where(d => d.EstimatedReadingTimeMinutes == filter.EstimatedReadingTimeMinutes);

    if (!string.IsNullOrWhiteSpace(filter.ImportanceLevel))
        query = query.Where(d => d.ImportanceLevel == filter.ImportanceLevel);

    if (!string.IsNullOrWhiteSpace(filter.Category))
        query = query.Where(d => d.Category == filter.Category);

    if (filter.IsActive != null)
        query = query.Where(d => d.IsActive == filter.IsActive);

    return await query.ToListAsync();
});

app.MapPost("/api/import-xml", async (AppDbContext db, HttpRequest request) =>
{
    var file = request.Form.Files.GetFile("file");

    if (file == null || file.Length == 0)
        return Results.BadRequest("No file uploaded");

    using var stream = file.OpenReadStream();
    using var reader = new StreamReader(stream);
    var xmlContent = await reader.ReadToEndAsync();

    var xml = XDocument.Parse(xmlContent);
    //var xml = XDocument.Load("backend/data/documents.xml");

    var docs = xml.Root!.Elements("document").Select(x => new DocumentMetadata
    {
        Title = (string)x.Element("title")!,
        Description = (string)x.Element("description")!,
        ResponsibleUnit = (string)x.Element("responsibleUnit")!,
        CreatedAt = DateTime.Parse((string)x.Element("createdAt")!),
        Url = (string)x.Element("url")!,
        FileType = (string)x.Element("fileType")!,
        EstimatedReadingTimeMinutes = int.Parse((string)x.Element("estimatedReadingTimeMinutes")!),
        ImportanceLevel = (string)x.Element("importanceLevel")!,
        Category = (string)x.Element("category")!,
        IsActive = bool.Parse((string)x.Element("isActive")!)
    });

    int imported = 0, updated = 0;
    foreach (var doc in docs)
    {
        var existing = await db.Documents
            .FirstOrDefaultAsync(d => d.Url == doc.Url);

        if (existing is null)
        {
            imported++;

            db.Documents.Add(doc);
        }
        else
        {
            updated++;

            existing.Title = doc.Title;
            existing.Description = doc.Description;
            existing.ResponsibleUnit = doc.ResponsibleUnit;
            existing.CreatedAt = doc.CreatedAt;
            existing.FileType = doc.FileType;
            existing.EstimatedReadingTimeMinutes = doc.EstimatedReadingTimeMinutes;
            existing.ImportanceLevel = doc.ImportanceLevel;
            existing.Category = doc.Category;
            existing.IsActive = doc.IsActive;
        }
    }

    await db.SaveChangesAsync();

    return Results.Ok(new { imported, updated });
});

app.Run();
