namespace CrawlDataWebsiteTool.Models
{
    public class ProductImageModel : AuditModel
    {
        public Guid? ProductId { get; set; }
        public string OriginLinkImage { get; set; }
        public string LocalPathImage { get; set; }
    }
}
