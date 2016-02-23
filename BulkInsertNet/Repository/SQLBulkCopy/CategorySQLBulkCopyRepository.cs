using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkInsertNET.Model;
using System.Data.SqlClient;
using System.Data;

namespace BulkInsertNet.Repository.SQLBulkCopy
{
    public class CategorySQLBulkCopyRepository : ICategoryRepository
    {
        private string categoriesTableName = "Categories";
        private string connectionstring;

        public CategorySQLBulkCopyRepository(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }


        public void BulkInsert(List<Category> categories)
        {

            // Connect to the database.
            using (var connection = new SqlConnection(this.connectionstring))
            {
                connection.Open();

                SqlTransaction transaction = null;
                DataTable categoriesTable = null;

                transaction = connection.BeginTransaction(); // Use one transaction to put all the data in the database

                try
                {
                    InsertCategories(categories, connection, transaction, categoriesTable);

                    int lastCategoryIdentity = SqlBulkCopyHelper.GetIdentity(categoriesTableName, connection, transaction);
                    int categoryId = lastCategoryIdentity - categories.Count();

                    foreach (var category in categories)
                    {
                        categoryId = categoryId + 1;
                        category.CategoryId = categoryId ;
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback(); // This will not reset IDENT_CURRENT
                    throw;
                }
                finally
                {
                    if (categoriesTable != null) { categoriesTable.Dispose(); }
                    if (transaction != null) { transaction.Dispose(); }
                }
            }
        }

        private void InsertCategories(IEnumerable<Category> categories,
                                    SqlConnection connection,
                                    SqlTransaction transaction,
                                    DataTable categoriesTable)
        {
            int productCount = categories.Count();
            categoriesTable = SqlBulkCopyHelper.GetSchemaInfo(categoriesTableName, connection, transaction);
            foreach (var category in categories)
            {
                DataRow row = categoriesTable.NewRow();

                //row["CategoryId"] = inventoryProduct.InventoryProductId;
                row["Name"] = category.Name;
                row["ExternalCode"] = category.ExternalCode;


                categoriesTable.Rows.Add(row);
            }
            SqlBulkCopyHelper.BulkInsertTable(categoriesTableName, categoriesTable, connection, transaction);

        }

    }
}
