using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

[Table("documents")]
public class DocumentMetadata
{
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = default!;

    [Column("description")]
    public string Description { get; set; } = default!;

    [Column("responsible_unit")]
    public string ResponsibleUnit { get; set; } = default!;

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("url")]
    public string Url { get; set; } = default!;

    [Column("file_type")]
    public string FileType { get; set; } = default!;

    [Column("estimated_reading_time_minutes")]
    public int EstimatedReadingTimeMinutes { get; set; }

    [Column("importance_level")]
    public string ImportanceLevel { get; set; } = default!;

    [Column("category")]
    public string Category { get; set; } = default!;

    [Column("is_active")]
    public bool IsActive { get; set; }
}