using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Datory;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesLayerSelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var sites = await _siteRepository.GetSitesAsync();
            foreach (var site in sites)
            {
                site.Set("SiteUrl", _pathManager.GetSiteUrl(site, false));
                site.Set("SiteTypeName", site.SiteType.GetDisplayName());
            }

            return new GetResult
            {
                Sites = sites,
            };
        }
    }
}