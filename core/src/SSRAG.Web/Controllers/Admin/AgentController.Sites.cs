using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpGet, Route(RouteSites)]
        public async Task<ActionResult<SitesResult>> Sites([FromQuery] AgentRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

            var sites = await _siteRepository.GetSitesAsync();
            foreach (var site in sites)
            {
                site.Set("LocalUrl", _pathManager.GetSiteUrl(site, true));
                site.Set("SiteUrl", _pathManager.GetSiteUrl(site, false));
            }

            var rootSiteId = await _siteRepository.GetIdByIsRootAsync();

            return new SitesResult
            {
                Sites = sites,
                RootSiteId = rootSiteId,
            };
        }
    }
}