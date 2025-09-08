using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Core.Utils;
using System;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<Site>>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsCreate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site.CreateStaticContentAddDate == DateTime.MinValue)
            {
                site.CreateStaticContentAddDate = DateTime.Now.AddYears(-10);
            }

            return new ObjectResult<Site>
            {
                Value = site
            };
        }
    }
}