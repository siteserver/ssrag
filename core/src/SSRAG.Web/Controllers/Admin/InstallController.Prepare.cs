using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RoutePrepare)]
        public async Task<ActionResult<StringResult>> Prepare([FromBody] PrepareRequest request)
        {
            if (!await _configRepository.IsNeedInstallAsync()) return Unauthorized();

            var (success, errorMessage) =
                await _administratorRepository.InsertValidateAsync(request.UserName, request.AdminPassword, request.Email, request.Mobile);

            if (!success)
            {
                return this.Error(errorMessage);
            }

            if (_settingsManager.Containerized)
            {
                if (_settingsManager.DatabaseType == DatabaseType.SQLite)
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath,
                        DbUtils.LocalDbContainerVirtualPath.Substring(1));
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        await FileUtils.WriteTextAsync(filePath, string.Empty);
                    }
                }
            }
            else
            {
                if (request.DatabaseType == DatabaseType.SQLite)
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, DbUtils.LocalDbHostVirtualPath.Substring(1));
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        await FileUtils.WriteTextAsync(filePath, string.Empty);
                    }
                }

                var databaseConnectionString = DbUtils.GetConnectionString(request.DatabaseType, request.DatabaseHost, request.IsDatabaseDefaultPort, TranslateUtils.ToInt(request.DatabasePort), request.DatabaseUserName, request.DatabasePassword, request.DatabaseName, request.DatabaseSchema);
                var redisConnectionString = string.Empty;
                if (request.IsRedis)
                {
                    redisConnectionString = Utilities.GetRedisConnectionString(request.RedisHost, request.IsRedisDefaultPort, request.RedisPort, request.RedisPassword, request.IsSsl, request.RedisDatabase, string.Empty);
                }

                _settingsManager.SaveSettings(request.IsProtectData, false, false, request.DatabaseType, databaseConnectionString,
                    redisConnectionString, string.Empty, null, null, false, null);
            }

            return new StringResult
            {
                Value = _settingsManager.SecurityKey
            };
        }
    }
}
