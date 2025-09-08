using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SSRAG.Datory.Caching;
using SqlKata;

namespace SSRAG.Datory.Utils
{
    internal static partial class RepositoryUtils
    {
        public static async Task<bool> ExistsAsync(IDatabase database, string tableName, IDistributedCache cache, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            if (compileInfo.Caching != null && compileInfo.Caching.Action == CachingAction.Get)
            {
                return await cache.GetOrCreateAsync(compileInfo.Caching.CacheKey,
                    async () => await _ExistsAsync(database, compileInfo)
                );
            }

            return await _ExistsAsync(database, compileInfo);
        }

        private static async Task<bool> _ExistsAsync(IDatabase database, CompileInfo compileInfo)
        {
            using var connection = database.GetConnection();
            return await connection.ExecuteScalarAsync<bool>(compileInfo.Sql, compileInfo.NamedBindings);
        }

        public static async Task<int> CountAsync(IDatabase database, string tableName, IDistributedCache cache, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            if (compileInfo.Caching != null && compileInfo.Caching.Action == CachingAction.Get)
            {
                var value = await cache.GetOrCreateAsync(
                    compileInfo.Caching.CacheKey,
                    async () => (await _CountAsync(database, compileInfo)).ToString()
                );
                return Utilities.ToInt(value);
            }

            return await _CountAsync(database, compileInfo);
        }

        private static async Task<int> _CountAsync(IDatabase database, CompileInfo compileInfo)
        {
            using var connection = database.GetConnection();
            return await connection.ExecuteScalarAsync<int>(compileInfo.Sql, compileInfo.NamedBindings);
        }

        public static async Task<TValue> SumValueAsync<TValue>(IDatabase database, string tableName, IDistributedCache cache, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsSum(columnName);
            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            if (compileInfo.Caching != null && compileInfo.Caching.Action == CachingAction.Get)
            {
                return await cache.GetOrCreateAsync(compileInfo.Caching.CacheKey,
                    async () => await _SumValueAsync<TValue>(database, compileInfo)
                );
            }

            return await _SumValueAsync<TValue>(database, compileInfo);
        }

        private static async Task<TValue> _SumValueAsync<TValue>(IDatabase database, CompileInfo compileInfo)
        {
            using var connection = database.GetConnection();
            return await connection.ExecuteScalarAsync<TValue>(compileInfo.Sql, compileInfo.NamedBindings);
        }

        public static async Task<TValue> GetValueAsync<TValue>(IDatabase database, string tableName, IDistributedCache cache, Query query)
        {
            if (query == null) return default;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            if (compileInfo.Caching != null && compileInfo.Caching.Action == CachingAction.Get)
            {
                return await cache.GetOrCreateAsync(compileInfo.Caching.CacheKey,
                    async () => await _GetValueAsync<TValue>(database, compileInfo)
                );
            }

            return await _GetValueAsync<TValue>(database, compileInfo);
        }

        private static async Task<TValue> _GetValueAsync<TValue>(IDatabase database, CompileInfo compileInfo)
        {
            using var connection = database.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<TValue>(compileInfo.Sql, compileInfo.NamedBindings);
        }

        public static async Task<List<TValue>> GetValueListAsync<TValue>(IDatabase database, string tableName, IDistributedCache cache, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            if (compileInfo.Caching != null && compileInfo.Caching.Action == CachingAction.Get)
            {
                return await cache.GetOrCreateAsync(compileInfo.Caching.CacheKey,
                    async () => await _GetValueListAsync<TValue>(database, compileInfo)
                );
            }

            return await _GetValueListAsync<TValue>(database, compileInfo);
        }

        private static async Task<List<TValue>> _GetValueListAsync<TValue>(IDatabase database, CompileInfo compileInfo)
        {
            using var connection = database.GetConnection();
            var list = await connection.QueryAsync<TValue>(compileInfo.Sql, compileInfo.NamedBindings);
            return list != null ? list.ToList() : new List<TValue>();
        }

        public static async Task<int> MaxAsync(IDatabase database, string tableName, IDistributedCache cache, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsMax(columnName);
            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            if (compileInfo.Caching != null && compileInfo.Caching.Action == CachingAction.Get)
            {
                return await cache.GetOrCreateAsync(compileInfo.Caching.CacheKey,
                    async () => await _MaxAsync(database, compileInfo)
                );
            }

            return await _MaxAsync(database, compileInfo);
        }

        private static async Task<int> _MaxAsync(IDatabase database, CompileInfo compileInfo)
        {
            using var connection = database.GetConnection();
            var max = await connection.QueryFirstOrDefaultAsync<int?>(compileInfo.Sql, compileInfo.NamedBindings);
            return max ?? 0;
        }

        public static async Task<T> GetObjectAsync<T>(IDatabase database, string tableName, IDistributedCache cache, Query query = null) where T : Entity, new()
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            if (compileInfo.Caching != null && compileInfo.Caching.Action == CachingAction.Get)
            {
                return await cache.GetOrCreateAsync(compileInfo.Caching.CacheKey,
                    async () => await _GetObjectAsync<T>(database, tableName, cache, compileInfo)
                );
            }

            return await _GetObjectAsync<T>(database, tableName, cache, compileInfo);
        }

        private static async Task<T> _GetObjectAsync<T>(IDatabase database, string tableName, IDistributedCache cache, CompileInfo compileInfo) where T : Entity, new()
        {
            dynamic row;
            T value = null;
            using (var connection = database.GetConnection())
            {
                row = await connection.QueryFirstOrDefaultAsync<dynamic>(compileInfo.Sql, compileInfo.NamedBindings);
            }

            if (row != null)
            {
                var fields = row as IDictionary<string, object>;
                value = new T();
                value.LoadDict(fields);
                await SyncAndCheckUuidAsync(database, tableName, cache, value);
            }

            return value;
        }

        public static async Task<List<T>> GetObjectListAsync<T>(IDatabase database, string tableName, IDistributedCache cache, Query query = null) where T : Entity, new()
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            if (compileInfo.Caching != null && compileInfo.Caching.Action == CachingAction.Get)
            {
                return await cache.GetOrCreateAsync(compileInfo.Caching.CacheKey,
                    async () => await _GetObjectListAsync<T>(database, tableName, cache, compileInfo)
                );
            }

            return await _GetObjectListAsync<T>(database, tableName, cache, compileInfo);
        }

        private static async Task<List<T>> _GetObjectListAsync<T>(IDatabase database, string tableName, IDistributedCache cache, CompileInfo compileInfo) where T : Entity, new()
        {
            IEnumerable<dynamic> results;
            var values = new List<T>();
            using (var connection = database.GetConnection())
            {
                results = await connection.QueryAsync<dynamic>(compileInfo.Sql, compileInfo.NamedBindings);
            }

            foreach (var row in results)
            {
                var fields = row as IDictionary<string, object>;
                var entity = new T();
                entity.LoadDict(fields);
                await SyncAndCheckUuidAsync(database, tableName, cache, entity);
                values.Add(entity);
            }

            return values;
        }
    }
}
