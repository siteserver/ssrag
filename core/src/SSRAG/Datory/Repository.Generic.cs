using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory.Utils;

namespace SSRAG.Datory
{
    public partial class Repository<T> : IRepository<T> where T : Entity, new()
    {
        public IDatabase Database { get; }
        public string TableName { get; }
        public List<TableColumn> TableColumns { get; private set; }
        public IDistributedCache Cache { get; }

        public Repository(IDatabase database)
        {
            Database = database;
            TableName = ReflectionUtils.GetTableName(typeof(T));
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
        }

        public Repository(IDatabase database, IDistributedCache cache)
        {
            Database = database;
            TableName = ReflectionUtils.GetTableName(typeof(T));
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
            Cache = cache;
        }

        public Repository(IDatabase database, string tableName)
        {
            Database = database;
            TableName = tableName;
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
        }

        public Repository(IDatabase database, string tableName, IDistributedCache cache)
        {
            Database = database;
            TableName = tableName;
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
            Cache = cache;
        }

        public async Task LoadTableColumnsAsync(string tableName)
        {
            TableColumns = await Database.GetTableColumnsAsync(tableName);
        }
    }
}
