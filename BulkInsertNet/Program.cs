using BulkInsertNet.Builders;
using BulkInsertNet.Database;
using BulkInsertNet.Repository;
using BulkInsertNet.Repository.Dapper;
using BulkInsertNet.Repository.SQLBulkCopy;
using BulkInsertNET.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsertNet
{
    class Program
    {
        static void Main(string[] args)
        {
            ICategoryRepository categoryRepository = null;
            IProductRepository productRepository = null;

            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BulkInsertNetConnectionString"].ConnectionString;
 
            if  (args.Count() == 0 || args[0] == "-sqlbulkcopy" )
            {
                Console.WriteLine("Process with SqlBulkCopy...");
                categoryRepository = new CategorySQLBulkCopyRepository(connectionString);
                productRepository = new ProductSQLBulkCopyRepository(connectionString);
            }
            else if (args[0] == "-dapper")
            {
                Console.WriteLine("Process with dapper...");
                categoryRepository = new CategoryDapperRepository(connectionString);
                productRepository = new ProductDapperRepository(connectionString);
            }

            // Delete data from db
            Helper.DeleteAllData(connectionString);

            // Build data
            var categories = CategoryBuilder.BuildCollection(100).ToList();
            var products = ProductBuilder.BuildCollection(10000, categories).ToList();


            InsertCategories(categoryRepository, categories);
            InsertProducts(productRepository, products);

            Console.WriteLine("End");
            Console.ReadLine();
        }

        static void InsertCategories(ICategoryRepository categoryRepository, IEnumerable<Category> categories)
        {
            Console.WriteLine("Inserting categories...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            categoryRepository.BulkInsert(categories.ToList());

            stopwatch.Stop();
            Console.WriteLine(String.Format("Time elapsed: {0}", stopwatch.Elapsed));
        }
        static void InsertProducts(IProductRepository productRepository, IEnumerable<Product> products)
        {
            Console.WriteLine("Inserting products...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            productRepository.BulkInsert(products.ToList());

            stopwatch.Stop();
            Console.WriteLine(String.Format("Time elapsed: {0}", stopwatch.Elapsed));
        }
    }
}
