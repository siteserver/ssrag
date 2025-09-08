using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Dto
{
    public class FlowNodeSql : FlowNode
    {
        public FlowNodeSql()
        {

        }

        public FlowNodeSql(FlowNode node)
        {
            var dict = node.ToDictionary();
            LoadDict(dict);
        }

        public bool IsSqlDatabase { get; set; }
        public DatabaseType SqlDatabaseType { get; set; }
        public string SqlDatabaseHost { get; set; }
        public bool IsSqlDatabasePort { get; set; }
        public string SqlDatabasePort { get; set; }
        public string SqlDatabaseUserName { get; set; }
        public string SqlDatabasePassword { get; set; }
        public string SqlDatabaseName { get; set; }
        public string SqlQueryString { get; set; }
    }
}
