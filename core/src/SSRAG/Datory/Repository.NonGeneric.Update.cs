using System;
using System.Threading.Tasks;
using SqlKata;
using SSRAG.Datory.Utils;

namespace SSRAG.Datory
{
    public partial class Repository
    {
        public virtual async Task<bool> UpdateAsync<T>(T dataInfo) where T : Entity
        {
            if (dataInfo == null || dataInfo.Id <= 0) return false;

            if (!Utilities.IsUuid(dataInfo.Uuid))
            {
                dataInfo.Uuid = Utilities.GetUuid();
            }
            dataInfo.LastModifiedDate = DateTime.Now;

            var query = Q.Where(nameof(Entity.Id), dataInfo.Id);

            foreach (var tableColumn in TableColumns)
            {
                if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id))) continue;

                var value = ValueUtils.GetSqlValue(dataInfo, tableColumn);

                query.Set(tableColumn.AttributeName, value);
            }

            return await RepositoryUtils.UpdateAllAsync(Database, TableName, Cache, query) > 0;
        }

        public virtual async Task<bool> UpdateAsync<T>(T dataInfo, params string[] columnNames) where T : Entity
        {
            if (dataInfo.Id > 0)
            {
                var query = Q.Where(nameof(Entity.Id), dataInfo.Id);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id))) continue;
                    query.Set(columnName, ValueUtils.GetValue(dataInfo, columnName));
                }

                return await RepositoryUtils.UpdateAllAsync(Database, TableName, Cache, query) > 0;
            }
            if (Utilities.IsUuid(dataInfo.Uuid))
            {
                var query = Q.Where(nameof(Entity.Uuid), dataInfo.Uuid);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id)) ||
                        Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Uuid))) continue;
                    query.Set(columnName, ValueUtils.GetValue(dataInfo, columnName));
                }

                return await RepositoryUtils.UpdateAllAsync(Database, TableName, Cache, query) > 0;
            }

            return false;
        }

        public virtual async Task<int> UpdateAsync(Query query)
        {
            return await RepositoryUtils.UpdateAllAsync(Database, TableName, Cache, query);
        }

        public virtual async Task<int> IncrementAsync(string columnName, Query query, int num = 1)
        {
            return await RepositoryUtils.IncrementAllAsync(Database, TableName, Cache, columnName, query, num);
        }

        public virtual async Task<int> DecrementAsync(string columnName, Query query, int num = 1)
        {
            return await RepositoryUtils.DecrementAllAsync(Database, TableName, Cache, columnName, query, num);
        }
    }
}
