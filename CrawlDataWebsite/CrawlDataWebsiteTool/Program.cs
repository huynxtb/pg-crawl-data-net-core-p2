using CrawlDataWebsiteTool.Repositories;
using CrawlDataWebsiteTool.Tools;
using System.Data.SqlClient;


/// <summary>
/// 
/// This project only support SQL Server
/// Data will be crawl from this website: https://fridayshopping.vn
/// You need to have basic knowledge with C#, Web HTML, CSS
/// If have any bug or question. Please comment in following this link: https://www.code-mega.com/p?q=crawl-data-trich-xuat-du-lieu-website-voi-c-phan-2-72953tZ
/// This project base on here: https://www.code-mega.com/p?q=crawl-data-trich-xuat-du-lieu-website-voi-c-phan-1-2c222jN
/// 
/// </summary>
/// 
/// <param name="connectionString">Your connection string</param>
/// <param name="currentPath">Get curent path of project</param>
/// <param name="customPathImage">If set, all crawled images will be saved to that path. Otherwise it will be set by default in the folder Product Image in the project</param>
/// <param name="path">Path to save crawled images</param>
///

const string connectionString = "Server=MRHUY\\SQLEXPRESS;Database=CrawlDataWatch;Trusted_Connection=True;";
const string customPathImage = "";

var currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
var path = string.IsNullOrEmpty(customPathImage) ? currentPath.Split("bin")[0] + @"Product Image\" : customPathImage;

try
{
    await using var connection = new SqlConnection(connectionString);
    
    var tool = new CodeMegaCrawler(path);

    // Crawl data 
    var dataInsert = tool.Crawling(5);

    var repo = new ProductRepository();

    // Begin insert to database
    await repo.InsertProductAsync(dataInsert, connection);

    Console.WriteLine($"Crawled {dataInsert.ListProduct.Count} Product(s)");
    Console.WriteLine($"Crawled {dataInsert.ListProductImage.Count} Images(s)");
    Console.WriteLine($"Crawled {dataInsert.ListProduct.Count} Property(s)");

    Console.WriteLine("DONE !!!");
}
catch (Exception e)
{
    throw;
}