namespace CrawlDataWebsiteTool.Models
{
    public class ProductModel : AuditModel
    {
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public double? Discount { get; set; }
        public string Currency { get; set; }
        public string OriginLinkDetail { get; set; }
    }
}
