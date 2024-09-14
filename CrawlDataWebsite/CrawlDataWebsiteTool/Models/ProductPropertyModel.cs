namespace CrawlDataWebsiteTool.Models
{
    public class ProductPropertyModel : AuditModel
    {
        public Guid? ProductId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
    }
}
