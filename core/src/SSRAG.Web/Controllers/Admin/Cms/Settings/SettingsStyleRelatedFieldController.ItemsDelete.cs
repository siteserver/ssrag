﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpPost, Route(RouteItemsDelete)]
        public async Task<ActionResult<ItemsResult>> ItemsDelete([FromBody] ItemsDeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            await _relatedFieldItemRepository.DeleteAsync(request.SiteId, request.Id);

            await _authManager.AddSiteLogAsync(request.SiteId, "删除联动字段项");

            var tree = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }
    }
}
