using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteInstall)]
        public async Task<ActionResult<BoolResult>> Install([FromBody] InstallRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey) ||
                string.IsNullOrEmpty(request.UserName) ||
                string.IsNullOrEmpty(request.Password))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }
            var (success, errorMessage) = await _administratorRepository.InsertValidateAsync(request.UserName, request.Password, string.Empty, string.Empty);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            if (await _configRepository.IsNeedInstallAsync())
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

                (success, errorMessage) = await _databaseManager.InstallAsync(request.UserName, request.Password, string.Empty, string.Empty);
                if (!success)
                {
                    return this.Error(errorMessage);
                }

                await FileUtils.WriteTextAsync(_pathManager.GetRootPath("index.html"), Constants.Html5Empty);
            }
            else
            {
                var admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
                if (admin != null)
                {
                    (success, errorMessage) = await _administratorRepository.ChangePasswordAsync(admin, request.Password);
                    if (!success)
                    {
                        return this.Error(errorMessage);
                    }
                    var cacheKey = Constants.GetSessionIdCacheKey(admin.UserName);
                    await _dbCacheRepository.RemoveAsync(cacheKey);
                }
                else
                {
                    admin = new Administrator
                    {
                        UserName = request.UserName,
                    };
                    (success, errorMessage) = await _administratorRepository.InsertAsync(admin, request.Password);
                    if (!success)
                    {
                        return this.Error(errorMessage);
                    }
                    await _administratorRepository.AddUserToRoleAsync(admin.UserName, PredefinedRole.ConsoleAdministrator.GetValue());
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}