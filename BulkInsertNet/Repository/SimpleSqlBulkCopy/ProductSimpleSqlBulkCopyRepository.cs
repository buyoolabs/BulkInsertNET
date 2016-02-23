using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkInsertNET.Model;
using System.Data.SqlClient;
using BulkInsertNet.Repository.SQLBulkCopy;

namespace BulkInsertNet.Repository.BulkInserter
{
    public class ProductSimpleSqlBulkCopyRepository : IProductRepository
    {
        private string productsTableName = "Products";
        private string productCategoriesTableName = "ProductCategories";

        private readonly string connectionString;
        public ProductSimpleSqlBulkCopyRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void BulkInsert(List<Product> products)
        {
            int lastProductIdentity = 0;
            InsertProducts(products);

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                SqlTransaction trans = cnn.BeginTransaction();
               lastProductIdentity = SqlBulkCopyHelper.GetIdentity(productsTableName, cnn, trans);
                trans.Commit();
                cnn.Close();
            }


            InsertProductCategories(products, lastProductIdentity);

           
        }

        private void InsertProducts(IEnumerable<Product> products)
        {
            var tunningProducts = from p in products select new { p.Title, p.Price, p.IsActive, p.Stock };
            using (var ssbc = new SimpleSqlBulkCopy(connectionString))
            {
                ssbc.WriteToServer(productsTableName, tunningProducts);
            }
        }

        private void InsertProductCategories(IEnumerable<Product> products)
        {
            using (var ssbc = new SimpleSqlBulkCopy(connectionString))
            {
                ssbc.WriteToServer(productsTableName, products);
            }
        }

        private void InsertProductCategories(IEnumerable<Product> products, int lastProductIdentity)
        {
            int productCount = products.Count();
            int productId = lastProductIdentity - productCount;

            List<ProductCategory> productCategories = new List<ProductCategory>();

            foreach (var product in products)
            {
                productId = productId + 1;

                foreach (var category in product.Categories)
                {
                    productCategories.Add(new ProductCategory() { ProductId = productId, CategoryId = category.CategoryId });
                }
            }

            using (var ssbc = new SimpleSqlBulkCopy(connectionString))
            {
                ssbc.WriteToServer(productCategoriesTableName, productCategories);
            }
        }
    }

    public class ProductCategory
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
    }
}
