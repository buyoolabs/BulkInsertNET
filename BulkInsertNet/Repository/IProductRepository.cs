using BulkInsertNET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsertNet.Repository
{
    interface IProductRepository
    {
        void BulkInsert(List<Product> products);
    }
}
