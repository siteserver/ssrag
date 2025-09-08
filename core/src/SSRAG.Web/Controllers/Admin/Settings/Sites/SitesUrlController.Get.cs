using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesUrlController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            var sites = await _siteRepository.GetSitesAsync();
            foreach (var site in sites)
            {
                site.Set("Web", _pathManager.GetSiteUrl(site, false));
                site.Set("Assets", _pathManager.GetAssetsUrl(site));
                site.Set("Api", _pathManager.GetApiHostUrl(site, Constants.ApiPrefix));
            }

            return new GetResult
            {
                Sites = sites
            };
        }
    }
}
