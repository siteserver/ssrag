using System.Collections.Generic;

namespace SSRAG.Datory
{
    public partial class Repository : IRepository
    {
        public IDatabase Database { get; }
        public string TableName { get; }
        public List<TableColumn> TableColumns { get; }
        public IDistributedCache Cache { get; }

        public Repository(IDatabase database)
        {
            Database = database;
            TableName = null;
            TableColumns = null;
        }

        public Repository(IDatabase database, IDistributedCache cache)
        {
            Database = database;
            TableName = null;
            TableColumns = null;
            Cache = cache;
        }

        public Repository(IDatabase database, string tableName)
        {
            Database = database;
            TableName = tableName;
            TableColumns = null;
        }

        public Repository(IDatabase database, string tableName, IDistributedCache cache)
        {
            Database = database;
            TableName = tableName;
            TableColumns = null;
            Cache = cache;
        }

        public Repository(IDatabase database, string tableName, List<TableColumn> tableColumns)
        {
            Database = database;
            TableName = tableName;
            TableColumns = tableColumns;
        }

        public Repository(IDatabase database, string tableName, List<TableColumn> tableColumns, IDistributedCache cache)
        {
            Database = database;
            TableName = tableName;
            TableColumns = tableColumns;
            Cache = cache;
        }
    }
}
