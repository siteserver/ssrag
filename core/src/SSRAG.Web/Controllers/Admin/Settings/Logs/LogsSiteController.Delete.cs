using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsSiteController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            await _siteLogRepository.DeleteAllAsync();

            await _authManager.AddAdminLogAsync("清空知识库日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
