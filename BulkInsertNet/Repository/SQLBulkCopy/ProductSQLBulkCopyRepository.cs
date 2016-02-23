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
    public class ProductSQLBulkCopyRepository : IProductRepository
    {

        private string productsTableName = "Products";
        private string productCategoriesTableName = "ProductCategories";
        private string connectionstring;

        public ProductSQLBulkCopyRepository(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }

        public void BulkInsert(List<Product> products)
        {

            // Connect to the database.
            using (var connection = new SqlConnection(this.connectionstring))
            {
                connection.Open();

                SqlTransaction transaction = null;
                DataTable productsTable = null;
                DataTable productCategoriesTable = null;


                transaction = connection.BeginTransaction(); // Use one transaction to put all the data in the database

                try
                {
                    InsertProducts(products, connection, transaction, productsTable);

                    int lastProductIdentity = SqlBulkCopyHelper.GetIdentity(productsTableName, connection, transaction);

                    InsertProductCategories(products, connection, transaction, productCategoriesTable, lastProductIdentity);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback(); // This will not reset IDENT_CURRENT
                    throw;
                }
                finally
                {
                    if (productsTable != null) { productsTable.Dispose(); }
                    if (productCategoriesTable != null) { productCategoriesTable.Dispose(); }
                    if (transaction != null) { transaction.Dispose(); }
                }
            }
        }

        private void InsertProducts(IEnumerable<Product> inventoryProducts,
                                    SqlConnection connection,
                                    SqlTransaction transaction,
                                    DataTable productsTable)
        {
            int productCount = inventoryProducts.Count();
            productsTable = SqlBulkCopyHelper.GetSchemaInfo(productsTableName, connection, transaction);
            foreach (var product in inventoryProducts)
            {
                DataRow row = productsTable.NewRow();

                //row["ProductId"] = inventoryProduct.InventoryProductId;
                row["Title"] = product.Title;
                row["Price"] = product.Price;
                row["Stock"] = product.Stock;
                row["Brand"] = product.Brand;
                row["IsActive"] = product.IsActive;

                productsTable.Rows.Add(row);
            }
            SqlBulkCopyHelper.BulkInsertTable(productsTableName, productsTable, connection, transaction);

        }

        private DataTable InsertProductCategories(IEnumerable<Product> products,
                            SqlConnection connection,
                            SqlTransaction transaction,
                            DataTable inventoryProductsTable,
                            int lastProductIdentity)
        {
            int productCount = products.Count();
            int productId = lastProductIdentity - productCount;

            DataTable productCategoriesTable = SqlBulkCopyHelper.GetSchemaInfo(productCategoriesTableName, connection, transaction);

            foreach (var product in products)
            {
                productId = productId + 1;

                foreach (var category in product.Categories)
                {
                    DataRow row = productCategoriesTable.NewRow();
                    row["ProductId"] = productId;
                    row["CategoryId"] = category.CategoryId;

                    productCategoriesTable.Rows.Add(row);
                }
            }
            SqlBulkCopyHelper.BulkInsertTable(productCategoriesTableName, productCategoriesTable, connection, transaction);
            return productCategoriesTable;
        }
    }
}
