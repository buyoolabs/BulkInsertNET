using BulkInsertNET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkInsertNet.Repository
{
    public interface ICategoryRepository
    {
        void BulkInsert(List<Category> categories);
    }
}
