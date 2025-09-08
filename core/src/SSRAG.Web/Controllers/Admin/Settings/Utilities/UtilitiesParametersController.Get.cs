using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SSRAG.Datory;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesParametersController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUtilitiesParameters))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            var environments = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("SSRAG 版本", $"SSRAG {_settingsManager.Version}"),
                new KeyValuePair<string, string>("最近升级时间", DateUtils.GetDateAndTimeString(config.UpdateDate)),
            };

            if (!_settingsManager.IsSafeMode)
            {
                environments.Add(new KeyValuePair<string, string>("运行环境", _settingsManager.Containerized ? "容器" : "主机"));
                environments.Add(new KeyValuePair<string, string>("OS 版本", _settingsManager.OSDescription));
                environments.Add(new KeyValuePair<string, string>("CPU Cores", _settingsManager.CPUCores.ToString()));
                environments.Add(new KeyValuePair<string, string>("系统主机名", Dns.GetHostName().ToUpper()));
            }

            var settings = new List<KeyValuePair<string, string>>();

            if (!_settingsManager.IsSafeMode)
            {
                settings.Add(new KeyValuePair<string, string>("系统根目录地址", _settingsManager.ContentRootPath));
                settings.Add(new KeyValuePair<string, string>("知识库根目录地址", _settingsManager.WebRootPath));
                settings.Add(new KeyValuePair<string, string>("数据库类型", _settingsManager.Database.DatabaseType.GetDisplayName()));
                settings.Add(new KeyValuePair<string, string>("数据库名称", _databaseManager.GetDatabaseNameFormConnectionString(_settingsManager.Database.ConnectionString)));
            }

            return new GetResult
            {
                Environments = environments,
                Settings = settings
            };
        }
    }
}
