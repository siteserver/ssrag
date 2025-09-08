﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormTemplatesLayerEditController
    {
        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormTemplates))
            {
                return Unauthorized();
            }

            if (request.Name == request.NameOriginal)
            {
                return new BoolResult
                {
                    Value = true
                };
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var templates = _formManager.GetFormTemplates(site);

            var originalTemplateInfo = templates.First(x => StringUtils.EqualsIgnoreCase(request.NameOriginal, x.Name));

            if (templates.Any(x => StringUtils.EqualsIgnoreCase(request.Name, x.Name)))
            {
                return this.Error($"标识为 {request.Name} 的模板已存在，请更换模板标识！");
            }

            var template = new FormTemplate
            {
                Name = request.Name,
            };
            templates.Add(template);

            _formManager.Clone(site, request.IsSystemOriginal, request.NameOriginal, request.Name);
            _formManager.DeleteTemplate(site, request.NameOriginal);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
