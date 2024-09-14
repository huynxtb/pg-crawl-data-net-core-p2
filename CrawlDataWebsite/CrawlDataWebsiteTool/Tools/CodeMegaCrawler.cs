using CrawlDataWebsiteTool.Functions;
using CrawlDataWebsiteTool.Helpers;
using CrawlDataWebsiteTool.Models;
using HtmlAgilityPack;
using System.Text;

namespace CrawlDataWebsiteTool.Tools
{
    /// <summary>
    /// 
    /// This class is used for crawling data
    /// Data will be crawl from this website: https://fridayshopping.vn
    /// 
    /// </summary>
    /// 
    /// <param name="BaseUrl">URL website need to crawl</param>
    /// <param name="_savePathImage">Path save image file</param>
    /// <param name="stopProduct">The total product you want to crawl. If not set it will be crawling all</param>
    /// 
    public class CodeMegaCrawler
    {
        private const string BaseUrl = "https://fridayshopping.vn";

        private readonly string _savePathImage;

        public CodeMegaCrawler(string savePathImage)
        {
            _savePathImage = savePathImage;
        }


        /// <summary>
        /// 
        /// This method is used for crawling data
        /// 
        /// </summary>
        public ProductDataFinalModel Crawling(int? stopProduct = null)
        {
            var dataCrawl = new ProductDataFinalModel();

            var web = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };

            var genders = new List<string>() { "nam", "nu" };

            Console.WriteLine("Please do not turn off the app while crawling!");

            foreach (var gender in genders)
            {
                var documentForGetTotalPageMale = web.Load(BaseUrl + $"/kieu-dang/{gender}/page/1");

                var textTotalPage = documentForGetTotalPageMale
                    .GetMultiNode(".page-numbers > li")
                    .Where(s => !string.IsNullOrEmpty(s.InnerText))
                    .ToList()
                    .Last()
                    .GetInnerText();

                int totalPage;
                int.TryParse(textTotalPage, out totalPage);

                for (var i = 1; i <= totalPage; i++)
                {
                    var requestPerPageMale = BaseUrl + $"/kieu-dang/{gender}/page/{i}";

                    var documentForListItem = web.Load(requestPerPageMale);

                    var listNodeProductItem = documentForListItem
                        .GetMultiNode("div.shop-container " +
                                      "> div.products " +
                                      "> div.product-small " +
                                      "> div.col-inner " +
                                      "> div.product-small " +
                                      "> div.box-image " +
                                      "> div.image-fade_in_back " +
                                      "> a");

                    var count = 0;

                    foreach (var href in listNodeProductItem)
                    {
                        if (count == stopProduct) return dataCrawl;

                        var detailLink = href.Attributes["href"].Value;

                        if (!string.IsNullOrEmpty(detailLink)
                            && detailLink.Contains("https://fridayshopping.vn"))
                        {
                            var documentForDetail = web.Load(detailLink);

                            if (documentForDetail != null)
                            {
                                var productId = Guid.NewGuid();

                                // Crawling product detail
                                dataCrawl.ListProduct.AddRange(BindingProductDetail(documentForDetail, productId, detailLink, dataCrawl));

                                // Crawling product property
                                dataCrawl.ListProductProperty.AddRange(BindingProductProperty(documentForDetail, productId, dataCrawl));

                                // Crawling product image
                                dataCrawl.ListProductImage.AddRange(BindingProductImage(documentForDetail, productId, dataCrawl));
                            }
                        }

                        count++;
                    }
                }
            }

            return dataCrawl;
        }

        /// <summary>
        /// 
        /// This method is used for crawling product detail
        /// 
        /// </summary>
        private IEnumerable<ProductModel> BindingProductDetail(HtmlDocument document, Guid productId, string detailLink,
            ProductDataFinalModel dataCrawl)
        {
            var spinner = new ConsoleSpinner();

            var productDetails = new List<ProductModel>();

            var textProductName = document.GetSingleNode("div.product-title-container > h1").GetInnerText();

            var textPrice = document
                ?.GetSingleNode("div.product-price-container")
                ?.GetInnerText()
                ?.ReplaceMultiToEmpty(new List<string>() { "₫", ",", "&#8363;" });

            var priceList = textPrice?.Split(" ").ToList();

            var orginPrice = priceList?.Count > 0 ? double.Parse(priceList[0]) : 0;

            var discountPrice = priceList?.Count > 0 ? double.Parse(priceList[1]) : 0;

            var discount = orginPrice - discountPrice;


            var dataInsertProductDetail = new ProductModel()
            {
                Id = productId,
                Currency = "VND",
                Discount = discount,
                OriginLinkDetail = detailLink,
                ProductName = textProductName,
                Price = orginPrice,
                Description = GetDescriptionForProductDetail(document)
            };

            spinner.Turn(displayMsg: $"Crawling {dataCrawl.ListProduct.Count} Products(s) | " +
                                     $"{dataCrawl.ListProductProperty.Count} Images(s) | " +
                                     $"{dataCrawl.ListProductImage.Count} Property(s) | ", sequenceCode: 5);

            productDetails.Add(dataInsertProductDetail);

            return productDetails;
        }

        /// <summary>
        /// 
        /// This method is used for crawling image
        /// 
        /// </summary>
        private IEnumerable<ProductImageModel> BindingProductImage(HtmlDocument document, Guid productId,
            ProductDataFinalModel dataCrawl)
        {
            var productImages = new List<ProductImageModel>();

            var productImageNodes = document
                ?.GetMultiNode("div.product-thumbnails " +
                               "> div.col " +
                               "> a " +
                               "> img");

            if (productImageNodes != null)
            {
                var spinner = new ConsoleSpinner();

                foreach (var item in productImageNodes)
                {
                    var src = item.Attributes["src"].Value;
                    var fileName = _savePathImage + DateTime.Now.Ticks + ".png";

                    if (!string.IsNullOrEmpty(src))
                    {
                        var pathSave = "";

                        if (src.StartsWith("https") || src.StartsWith("http"))
                        {
                            var pathReturn = DownloadHelper.DownloadImageFromUri(src, fileName);

                            if (!string.IsNullOrEmpty(pathReturn))
                            {
                                pathSave = pathReturn;
                            }
                        }
                        else
                        {
                            if (src.StartsWith("/"))
                            {
                                src = BaseUrl + src;
                            }
                            else
                            {
                                if (src.StartsWith(".") || src.StartsWith("~")) src = BaseUrl + src.Substring(1);
                                else src = BaseUrl + "/" + src;
                            }

                            var pathReturn = DownloadHelper.DownloadImageFromUri(src, fileName);

                            if (!string.IsNullOrEmpty(pathReturn))
                            {
                                pathSave = pathReturn;
                            }
                        }

                        if (!string.IsNullOrEmpty(pathSave))
                        {
                            var dataInsertProductImage = new ProductImageModel()
                            {
                                Id = Guid.NewGuid(),
                                ProductId = productId,
                                OriginLinkImage = src,
                                LocalPathImage = fileName
                            };

                            productImages.Add(dataInsertProductImage);

                            spinner.Turn(displayMsg: $"Crawling {dataCrawl.ListProduct.Count} Products(s) | " +
                                                     $"{dataCrawl.ListProductProperty.Count} Images(s) | " +
                                                     $"{dataCrawl.ListProductImage.Count} Property(s) | ", sequenceCode: 5);
                        }
                    }
                }
            }

            return productImages;
        }

        /// <summary>
        /// 
        /// This method is used for crawling product property
        /// 
        /// </summary>
        private IEnumerable<ProductPropertyModel> BindingProductProperty(HtmlDocument document, Guid productId,
            ProductDataFinalModel dataCrawl)
        {
            var productProperties = new List<ProductPropertyModel>();

            var textDescriptionPTag = document
                ?.GetMultiNode("div.product-short-description p");

            if (textDescriptionPTag != null)
            {
                var spinner = new ConsoleSpinner();

                foreach (var item in textDescriptionPTag)
                {
                    if (!string.IsNullOrEmpty(item.GetInnerText()))
                    {
                        if (item.InnerHtml.StartsWith("<strong>"))
                        {
                            var textProperty = item.GetInnerText();
                            var properties = textProperty?.Split(":").ToList();

                            if (properties?.Count == 2)
                            {
                                var propertyName = properties.First().RemoveBreakLineTab();
                                var propertyValue = properties.Last().RemoveBreakLineTab();

                                if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
                                {
                                    var dataInsertProductProperty = new ProductPropertyModel()
                                    {
                                        Id = Guid.NewGuid(),
                                        ProductId = productId,
                                        PropertyName = propertyName,
                                        PropertyValue = propertyValue
                                    };

                                    productProperties.Add(dataInsertProductProperty);

                                    spinner.Turn(displayMsg: $"Crawling {dataCrawl.ListProduct.Count} Products(s) | " +
                                                             $"{dataCrawl.ListProductProperty.Count} Images(s) | " +
                                                             $"{dataCrawl.ListProductImage.Count} Property(s) | ", sequenceCode: 5);
                                }
                            }
                        }
                    }
                }
            }

            // Get all information in section table

            var textDescriptionTableTag = document
                ?.GetMultiNode("div.product-short-description > table > tbody > tr");

            if (textDescriptionTableTag != null)
            {
                var spinner = new ConsoleSpinner();

                foreach (var item in textDescriptionTableTag)
                {
                    if (!string.IsNullOrEmpty(item.GetInnerText()))
                    {
                        var textProperty = item.GetInnerText();
                        var properties = textProperty?.Split(":").ToList();

                        if (properties?.Count == 2)
                        {
                            var propertyName = properties.First().RemoveBreakLineTab();
                            var propertyValue = properties.Last().RemoveBreakLineTab();

                            if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
                            {
                                var dataInsertProductProperty = new ProductPropertyModel()
                                {
                                    Id = Guid.NewGuid(),
                                    ProductId = productId,
                                    PropertyName = propertyName,
                                    PropertyValue = propertyValue
                                };

                                productProperties.Add(dataInsertProductProperty);

                                spinner.Turn(displayMsg: $"Crawling {dataCrawl.ListProduct.Count} Products(s) | " +
                                                         $"{dataCrawl.ListProductProperty.Count} Images(s) | " +
                                                         $"{dataCrawl.ListProductImage.Count} Property(s) | ", sequenceCode: 5);
                            }
                        }
                    }
                }
            }

            return productProperties;
        }

        /// <summary>
        /// 
        /// This method is used for get description for product detail
        /// 
        /// </summary>
        private string GetDescriptionForProductDetail(HtmlDocument document)
        {
            var textDescriptionPTag = document
                ?.GetMultiNode("div.product-short-description p");

            if (textDescriptionPTag != null)
            {
                foreach (var item in textDescriptionPTag)
                {
                    if (!string.IsNullOrEmpty(item.GetInnerText()))
                    {
                        if (!item.InnerHtml.StartsWith("<strong>"))
                        {
                            return item.GetInnerText();
                        }
                    }
                }
            }

            return "";
        }
    }
}