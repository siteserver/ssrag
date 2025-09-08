using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormDataController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] FormRequest request)
        {
            var formPermission = MenuUtils.GetFormPermission(request.FormId);
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var styles = await _formRepository.GetTableStylesAsync(form.Id);
            var logs = await _formDataRepository.GetListAsync(form);

            var head = new List<string> { "编号" };
            foreach (var style in styles)
            {
                head.Add(style.DisplayName);
            }
            head.Add("添加时间");

            var rows = new List<List<string>>();

            foreach (var log in logs)
            {
                var row = new List<string>
                {
                    log.Uuid
                };
                foreach (var style in styles)
                {
                    row.Add(_formDataRepository.GetValue(style, log));
                }

                if (log.CreatedDate.HasValue)
                {
                    row.Add(log.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm"));
                }

                rows.Add(row);
            }

            var fileName = $"{form.Title}.xlsx";
            ExcelUtils.Write(_pathManager.GetTemporaryFilesPath(fileName), head, rows);
            var downloadUrl = _pathManager.GetTemporaryFilesUrl(fileName);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
