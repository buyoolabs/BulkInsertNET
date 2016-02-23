using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsertNet.Database
{
    public static class Helper
    {

        public static void DeleteAllData(string connectionString)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                cnn.Execute(@"DELETE ProductCategories");
                cnn.Execute(@"DELETE Categories");
                cnn.Execute(@"DELETE Products");
                cnn.Close();
            }
        }
    }
}
