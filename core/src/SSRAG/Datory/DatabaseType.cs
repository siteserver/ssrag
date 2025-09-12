using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SSRAG.Datory.Annotations;

namespace SSRAG.Datory
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DatabaseType
    {
        [DataEnum(DisplayName = "MySql")] MySql,
        [DataEnum(DisplayName = "MariaDB")] MariaDB,
        [DataEnum(DisplayName = "SqlServer")] SqlServer,
        [DataEnum(DisplayName = "PostgreSQL")] Postgres,
        [DataEnum(DisplayName = "SQLite")] SQLite,
        [DataEnum(DisplayName = "人大金仓")] Kingbase,
        [DataEnum(DisplayName = "达梦")] Dm,
        [DataEnum(DisplayName = "OceanBase")] OceanBase,
    }
}