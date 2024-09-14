namespace CrawlDataWebsiteTool.Models
{
    public class AuditModel
    {
        public Guid Id { get; set; } = new();
        public bool IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
