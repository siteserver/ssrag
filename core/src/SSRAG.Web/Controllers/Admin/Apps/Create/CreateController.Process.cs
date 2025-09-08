using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Apps.Create
{
    public partial class CreateController
    {
        [HttpPost, Route(RouteProcess)]
        public async Task<ActionResult<Process>> Process([FromBody] ProcessRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            var caching = new Caching(_settingsManager);
            return await caching.GetProcessAsync(request.Uuid);
        }
    }
}