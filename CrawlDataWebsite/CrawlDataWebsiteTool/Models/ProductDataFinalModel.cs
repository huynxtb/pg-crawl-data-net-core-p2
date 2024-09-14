namespace CrawlDataWebsiteTool.Models
{
    public class ProductDataFinalModel
    {
        public ProductDataFinalModel()
        {
            ListProduct = new List<ProductModel>();
            ListProductProperty = new List<ProductPropertyModel>();
            ListProductImage = new List<ProductImageModel>();
        }

        public List<ProductModel> ListProduct { get; set; }
        public List<ProductPropertyModel> ListProductProperty { get; set; }
        public List<ProductImageModel> ListProductImage { get; set; }
    }
}
