using System.Threading.Tasks;
using SqlKata;
using SSRAG.Datory.Utils;

namespace SSRAG.Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual async Task<bool> DeleteAsync(int id, Query query = null)
        {
            if (id <= 0) return false;

            query ??= Q.NewQuery();

            return await DeleteAsync(query.Where(nameof(Entity.Id), id)) > 0;
        }

        public virtual async Task<bool> DeleteAsync(string uuid, Query query = null)
        {
            if (!Utilities.IsUuid(uuid)) return false;

            query ??= Q.NewQuery();

            return await DeleteAsync(query.Where(nameof(Entity.Uuid), uuid)) > 0;
        }

        public virtual async Task<int> DeleteAsync(Query query = null)
        {
            return await RepositoryUtils.DeleteAllAsync(Database, TableName, Cache, query);
        }
    }
}
