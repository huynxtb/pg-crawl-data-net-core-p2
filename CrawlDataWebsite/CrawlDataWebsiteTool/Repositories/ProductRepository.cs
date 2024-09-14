using CrawlDataWebsiteTool.Models;
using Dapper;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace CrawlDataWebsiteTool.Repositories
{
    public class ProductRepository
    {
        /// <summary>
        /// 
        /// This class is used for insert data to Database
        /// 
        /// </summary>
        public async Task<bool> InsertProductAsync(ProductDataFinalModel data, SqlConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@JInputProductDetail", JsonConvert.SerializeObject(data.ListProduct));
                parameters.Add("@JInputProductProperty", JsonConvert.SerializeObject(data.ListProductProperty));
                parameters.Add("@JInputProductImage", JsonConvert.SerializeObject(data.ListProductImage));

                var result = await connection.ExecuteAsync("spInsUpdCrawlDataWebsite", parameters, commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
