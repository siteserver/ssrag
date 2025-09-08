using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Repositories;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormStylesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] FormRequest request)
        {
            var formPermission = MenuUtils.GetFormPermission(request.FormId);
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var styles = await _formRepository.GetTableStylesAsync(form.Id);

            var inputTypes = ListUtils.GetSelects<InputType>();

            return new GetResult
            {
                InputTypes = inputTypes,
                TableName = FormDataRepository.TABLE_NAME,
                RelatedIdentities = ListUtils.ToString(_formRepository.GetRelatedIdentities(form.Id)),
                Styles = styles,
            };
        }
    }
}
