using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteDatabaseConnect)]
        public async Task<ActionResult<DatabaseConnectResult>> DatabaseConnect([FromBody] DatabaseConnectRequest request)
        {
            if (!await _configRepository.IsNeedInstallAsync()) return Unauthorized();

            var databaseType = _settingsManager.Containerized ? _settingsManager.DatabaseType : request.DatabaseType;
            var databaseName = (databaseType == DatabaseType.Dm || databaseType == DatabaseType.Kingbase) ? request.DatabaseName : string.Empty;
            var connectionString = _settingsManager.Containerized
                ? _settingsManager.DatabaseConnectionString
                : DbUtils.GetConnectionString(request.DatabaseType, request.DatabaseHost,
                    request.IsDatabaseDefaultPort, TranslateUtils.ToInt(request.DatabasePort), request.DatabaseUserName,
                    request.DatabasePassword, databaseName, string.Empty);

            var db = new Database(databaseType, connectionString);

            var (isConnectionWorks, message) = await db.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                return this.Error(message);
            }

            var databaseNames = new List<string>();

            if (string.IsNullOrEmpty(databaseName))
            {
                databaseNames = await db.GetDatabaseNamesAsync();
            }

            // if (databaseType == DatabaseType.Dm)
            // {
            //     if (ListUtils.ContainsIgnoreCase(databaseNames, request.DatabaseUserName))
            //     {
            //         databaseNames = new List<string>
            //         {
            //             request.DatabaseUserName
            //         };
            //     }
            // }

            return new DatabaseConnectResult
            {
                DatabaseNames = databaseNames
            };
        }
    }
}
