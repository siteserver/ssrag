using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesCacheController
    {
        [HttpPost, Route(RouteClearCache)]
        public async Task<ActionResult<BoolResult>> ClearCache()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUtilitiesCache))
            {
                return Unauthorized();
            }

            var temporaryFilesPath = _pathManager.GetTemporaryFilesPath();
            DirectoryUtils.DeleteDirectoryIfExists(temporaryFilesPath);
            DirectoryUtils.CreateDirectoryIfNotExists(temporaryFilesPath);

            await _dbCacheRepository.ClearAllExceptAdminSessionsAsync();
            await _settingsManager.Cache.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}