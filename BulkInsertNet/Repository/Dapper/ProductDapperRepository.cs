using BulkInsertNET.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsertNet.Repository.Dapper
{
    public class ProductDapperRepository:IProductRepository
    {
        private readonly string connectionString;
        public ProductDapperRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void BulkInsert(List<Product> products)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                foreach (var product in products)
                {
                    var productId = cnn.Query<int>(@"INSERT Products([Title],[Price],[Stock],[Brand],[IsActive]) VALUES (@Title,@Price,@Stock,@Brand,@IsActive) 
                                SELECT ProductId FROM Products WHERE ProductId = scope_identity()", product).First();
                    foreach(var category in product.Categories)
                    {
                        cnn.Execute(@"INSERT ProductCategories(ProductId,CategoryId) VALUES (@ProductId,@CategoryId)", new { ProductId = productId, CategoryId = category.CategoryId });
                    }
                }
                cnn.Close();
            }
        }
    }
}
