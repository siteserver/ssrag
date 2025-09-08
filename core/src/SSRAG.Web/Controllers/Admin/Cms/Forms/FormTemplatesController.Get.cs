using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormTemplatesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormTemplates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var forms = await _formRepository.GetFormsAsync(request.SiteId);
            var templates = _formManager.GetFormTemplates(site);
            var siteUrl = _pathManager.GetSiteUrl(site, true);

            return new GetResult
            {
                Forms = forms,
                Templates = templates,
                SiteUrl = siteUrl,
            };
        }
    }
}
