using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsSiteController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SiteLogPageResult>> Get([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            return await GetResultsAsync(request);
        }
    }
}
