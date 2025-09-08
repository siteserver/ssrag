using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [HttpGet, Route(RouteId)]
        public async Task<ActionResult<GetSpecialResult>> GetSpecial(int siteId, int specialId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId,
                MenuUtils.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            Special special = null;
            if (specialId > 0)
            {
                special = await _specialRepository.GetSpecialAsync(siteId, specialId);
            }

            return new GetSpecialResult
            {
                Special = special,
                Uuid = StringUtils.GetShortUuid(false),
            };
        }
    }
}