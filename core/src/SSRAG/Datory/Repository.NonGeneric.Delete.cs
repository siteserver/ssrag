using System.Threading.Tasks;
using SqlKata;
using SSRAG.Datory.Utils;

namespace SSRAG.Datory
{
    public partial class Repository
    {
        public virtual async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) return false;

            return await DeleteAsync(Q.Where(nameof(Entity.Id), id)) > 0;
        }

        public virtual async Task<bool> DeleteAsync(string uuid)
        {
            if (!Utilities.IsUuid(uuid)) return false;

            return await DeleteAsync(Q.Where(nameof(Entity.Uuid), uuid)) > 0;
        }

        public virtual async Task<int> DeleteAsync(Query query = null)
        {
            return await RepositoryUtils.DeleteAllAsync(Database, TableName, Cache, query);
        }
    }
}
