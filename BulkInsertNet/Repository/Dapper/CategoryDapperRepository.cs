using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkInsertNET.Model;
using System.Data.SqlClient;
using Dapper;

namespace BulkInsertNet.Repository.Dapper
{
    public class CategoryDapperRepository : ICategoryRepository
    {
        private readonly string connectionString;
        public CategoryDapperRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public void BulkInsert(List<Category> categories)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                for (int i = 0; i < categories.Count(); i++)
                {
                    categories[i].CategoryId = cnn.Query<int>(@"INSERT Categories(Name,ExternalCode) VALUES (@Name,@ExternalCode)
                                                            SELECT CategoryId FROM Categories WHERE CategoryId = SCOPE_IDENTITY()", categories[i]).First();
                }
                cnn.Close();
            }
        }
    }
}
