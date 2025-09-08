using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory.Utils;
using SqlKata;

namespace SSRAG.Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual async Task<int> InsertAsync(T dataInfo, Query query = null)
        {
            return await RepositoryUtils.InsertObjectAsync(Database, TableName, TableColumns, Cache, dataInfo, query);
        }

        public virtual async Task BulkInsertAsync(IEnumerable<T> items)
        {
            await RepositoryUtils.BulkInsertAsync(Database, TableName, TableColumns, items);
        }
    }
}
