using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsWaterMarkController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsWaterMark))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var families = FontUtils.GetFontFamilies();
            var imageUrl = _pathManager.ParseSiteUrl(site, site.WaterMarkImagePath, true);

            return new GetResult
            {
                Site = site,
                Families = families,
                ImageUrl = imageUrl
            };
        }
    }
}