using System;

namespace DocumentsApi
{
    public class DocumentFilter
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ResponsibleUnit { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Url { get; set; }
        public string? FileType { get; set; }
        public int? EstimatedReadingTimeMinutes { get; set; }
        public string? ImportanceLevel { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
    }
}
