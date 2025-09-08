using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata;
using SSRAG.Datory.Utils;

namespace SSRAG.Datory
{
    public partial class Repository
    {
        public virtual async Task<T> GetAsync<T>(int id) where T : Entity
        {
            return id <= 0 ? null : await GetAsync<T>(Q.Where(nameof(Entity.Id), id));
        }

        public virtual async Task<T> GetAsync<T>(string uuid) where T : Entity
        {
            return !Utilities.IsUuid(uuid) ? null : await GetAsync<T>(Q.Where(nameof(Entity.Uuid), uuid));
        }

        public virtual async Task<T> GetAsync<T>(Query query = null)
        {
            var value = await RepositoryUtils.GetValueAsync<T>(Database, TableName, Cache, query);

            if (typeof(T).IsAssignableFrom(typeof(Entity)))
            {
                await RepositoryUtils.SyncAndCheckUuidAsync(Database, TableName, Cache, value as Entity);
            }

            return value;
        }

        public virtual async Task<List<T>> GetAllAsync<T>(Query query = null)
        {
            var list = await RepositoryUtils.GetValueListAsync<T>(Database, TableName, Cache, query);

            if (typeof(T).IsAssignableFrom(typeof(Entity)))
            {
                foreach (var value in list)
                {
                    await RepositoryUtils.SyncAndCheckUuidAsync(Database, TableName, Cache, value as Entity);
                }
            }

            return list;
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return id > 0 && await ExistsAsync(Q.Where(nameof(Entity.Id), id));
        }

        public virtual async Task<bool> ExistsAsync(string uuid)
        {
            return Utilities.IsUuid(uuid) && await ExistsAsync(Q.Where(nameof(Entity.Uuid), uuid));
        }

        public virtual async Task<bool> ExistsAsync(Query query = null)
        {
            return await RepositoryUtils.ExistsAsync(Database, TableName, Cache, query);
        }

        public virtual async Task<int> CountAsync(Query query = null)
        {
            return await RepositoryUtils.CountAsync(Database, TableName, Cache, query);
        }

        public virtual async Task<int> SumAsync(string columnName, Query query = null)
        {
            return await RepositoryUtils.SumValueAsync<int>(Database, TableName, Cache, columnName, query);
        }

        public virtual async Task<TValue> SumAsync<TValue>(string columnName, Query query = null)
        {
            return await RepositoryUtils.SumValueAsync<TValue>(Database, TableName, Cache, columnName, query);
        }

        public virtual async Task<int?> MaxAsync(string columnName, Query query = null)
        {
            return await RepositoryUtils.MaxAsync(Database, TableName, Cache, columnName, query);
        }
    }
}
