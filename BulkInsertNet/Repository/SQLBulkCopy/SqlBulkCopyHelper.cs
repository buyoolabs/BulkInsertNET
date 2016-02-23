using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsertNet.Repository.SQLBulkCopy
{
    public static class SqlBulkCopyHelper
    {
        public static DataTable GetSchemaInfo(string tableName, SqlConnection connection, SqlTransaction transaction)
        {
            DataTable dataTable = new DataTable();
            using (SqlCommand selectSchemaCommand = connection.CreateCommand())
            {
                selectSchemaCommand.CommandText = string.Format("set fmtonly on; select * from {0}", tableName);
                selectSchemaCommand.Transaction = transaction;

                using (var adapter = new SqlDataAdapter(selectSchemaCommand)) // Get only the schema information for table [Sale]
                {
                    adapter.FillSchema(dataTable, SchemaType.Source);
                }
            }
            return dataTable;
        }
        public static int GetIdentity(string tableName, SqlConnection connection, SqlTransaction transaction)
        {
            int identity = 0;

            // Get the last customer identity
            using (SqlCommand sqlCommand = connection.CreateCommand())
            {
                sqlCommand.CommandText = string.Format("SELECT IDENT_CURRENT('{0}')", tableName);
                sqlCommand.Transaction = transaction;
                identity = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }

            return identity;
        }
        public static void BulkInsertTable(string tableName, DataTable dataTable, SqlConnection connection, SqlTransaction transaction)
        {
            using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)) // Lock the table
            {
                sqlBulkCopy.BulkCopyTimeout = 500;
                sqlBulkCopy.DestinationTableName = tableName;
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }
    }
}
