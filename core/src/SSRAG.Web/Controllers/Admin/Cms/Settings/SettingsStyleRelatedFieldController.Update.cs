﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Models;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<IEnumerable<RelatedField>>> Edit([FromBody] RelatedField request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            await _relatedFieldRepository.UpdateAsync(request);

            await _authManager.AddSiteLogAsync(request.SiteId, "编辑联动字段");

            return await _relatedFieldRepository.GetRelatedFieldsAsync(request.SiteId);
        }
    }
}
