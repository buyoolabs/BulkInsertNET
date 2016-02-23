using BulkInsertNET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsertNet.Builders
{
    public static class CategoryBuilder
    {
        public static IEnumerable<Category> BuildCollection(int numberOfItems)
        {
            for(int i=0; i< numberOfItems; i++)
            {
                var category = new Category() { Name = string.Format("Sample {0}", i), ExternalCode = string.Format("QWERTY{0}", i) };
                yield return category;
            }
        }
    }
}
