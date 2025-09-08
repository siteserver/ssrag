using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata;

[assembly: InternalsVisibleTo("Datory.Data.Tests")]

namespace SSRAG.Datory.Utils
{
    internal static partial class RepositoryUtils
    {
        public static async Task SyncAndCheckUuidAsync(IDatabase database, string tableName, IDistributedCache cache, Entity dataInfo)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return;

            dataInfo.LoadExtend();

            if (Utilities.IsUuid(dataInfo.Uuid)) return;

            dataInfo.Uuid = Utilities.GetUuid();
            dataInfo.LastModifiedDate = DateTime.Now;

            await UpdateAllAsync(database, tableName, cache, new Query()
                .Set(nameof(Entity.Uuid), dataInfo.Uuid)
                .Where(nameof(Entity.Id), dataInfo.Id)
            );
        }

        public static async Task<int> UpdateAllAsync(IDatabase database, string tableName, IDistributedCache cache, Query query)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.Method = "update";

            var compileInfo = await CompileAsync(database, tableName, cache, xQuery);

            using var connection = database.GetConnection();
            return await connection.ExecuteAsync(compileInfo.Sql, compileInfo.NamedBindings);
        }

        public static async Task<int> IncrementAllAsync(IDatabase database, string tableName, IDistributedCache cache, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{database.GetQuotedIdentifier(columnName)} = {DbUtils.ColumnIncrement(database.DatabaseType, columnName, num)}");

            return await UpdateAllAsync(database, tableName, cache, xQuery);
        }

        public static async Task<int> DecrementAllAsync(IDatabase database, string tableName, IDistributedCache cache, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{database.GetQuotedIdentifier(columnName)} = {DbUtils.ColumnDecrement(database.DatabaseType, columnName, num)}");

            return await UpdateAllAsync(database, tableName, cache, xQuery);
        }
    }
}
