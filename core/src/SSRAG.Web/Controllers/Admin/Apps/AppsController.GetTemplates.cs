using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Apps
{
    public partial class AppsController
    {
        [HttpPost, Route(RouteGetTemplates)]
        public async Task<ActionResult<GetTemplatesResult>> Get([FromBody] GetTemplatesRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            var caching = new Caching(_settingsManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching, request.SiteType);
            var siteTemplates = manager.GetSiteTemplates();

            return new GetTemplatesResult
            {
                SiteTemplates = siteTemplates,
            };
        }
    }
}