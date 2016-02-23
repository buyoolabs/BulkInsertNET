using BulkInsertNET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsertNet.Builders
{
    public static class ProductBuilder
    {
        public static IEnumerable<Product> BuildCollection(int numberOfItems, IEnumerable<Category> categories)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                var product = new Product() { Title = string.Format("My Product {0}", i), Price = 2.68m * i, Stock = i + 8, IsActive = true, Brand = string.Format("My Brand {0}", i), Categories = categories };
                yield return product;
            }
        }
    }
}
