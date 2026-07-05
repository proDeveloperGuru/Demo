using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

public class DocumentsApiTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DocumentsApiTests()
    {
        var app = new WebApplicationFactory<Program>();
        _client = app.CreateClient();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Clear existing test records to ensure clean slate
        db.Documents.RemoveRange(db.Documents);

        db.Documents.AddRange(new List<DocumentMetadata>
        {
            new() { Id = 1, Title = "Annual Report", Category = "Finance", Url = "http://test.com/1", Description = "Test", ResponsibleUnit = "HR", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Title = "Tech Spec", Category = "IT", Url = "http://test.com/2", Description = "Test", ResponsibleUnit = "Dev", CreatedAt = DateTime.UtcNow }
        });
        db.SaveChanges();
    }

    [Fact]
    public async Task GetDocuments()
    {
        var response = await _client.GetAsync("/api/documents");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ImportXml()
    {
        var xmlContent = @"
        <documents>
            <document>
                <title>New Imported Doc</title>
                <description>From XML</description>
                <responsibleUnit>Security</responsibleUnit>
                <createdAt>2026-07-05T12:00:00</createdAt>
                <url>http://test.com/new-xml</url>
                <fileType>pdf</fileType>
                <estimatedReadingTimeMinutes>5</estimatedReadingTimeMinutes>
                <importanceLevel>High</importanceLevel>
                <category>Legal</category>
                <isActive>true</isActive>
            </document>
        </documents>";

        var multipartContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(xmlContent));
        multipartContent.Add(fileContent, "file", "documents.xml");

        var response = await _client.PostAsync("/api/import-xml", multipartContent);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        Assert.NotNull(result);
        Assert.Equal(1, result["imported"]);
        Assert.Equal(0, result["updated"]);
    }
}
