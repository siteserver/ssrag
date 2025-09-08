﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Models;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<IEnumerable<RelatedField>>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            await _relatedFieldRepository.DeleteAsync(request.RelatedFieldId);

            await _authManager.AddSiteLogAsync(request.SiteId, "删除联动字段");

            return await _relatedFieldRepository.GetRelatedFieldsAsync(request.SiteId);
        }
    }
}
