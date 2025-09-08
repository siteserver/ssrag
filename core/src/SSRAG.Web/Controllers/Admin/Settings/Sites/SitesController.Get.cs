using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Utils;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var rootSiteId = await _siteRepository.GetIdByIsRootAsync();
            var siteTypes = ListUtils.GetEnums<SiteType>();
            var sites = await _siteRepository.GetSitesAsync();
            var tableNames = await _siteRepository.GetSiteTableNamesAsync();

            return new GetResult
            {
                SiteTypes = siteTypes,
                Sites = sites,
                RootSiteId = rootSiteId,
                TableNames = tableNames,
            };
        }
    }
}