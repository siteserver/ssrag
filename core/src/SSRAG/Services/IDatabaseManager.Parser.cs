using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SqlKata;

namespace SSRAG.Services
{
    public partial interface IDatabaseManager
    {
        Task<List<KeyValuePair<int, IDictionary<string, object>>>> ParserGetSqlDataSourceAsync(
            DatabaseType databaseType, string connectionString, string queryString);

        Task<List<KeyValuePair<int, IDictionary<string, object>>>> ParserGetSqlDataSourceAsync(
            DatabaseType databaseType, string connectionString, Query query);
    }
}
