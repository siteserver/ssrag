﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsChannelGroupController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsChannelGroup))
            {
                return Unauthorized();
            }

            await _channelGroupRepository.DeleteAsync(request.SiteId, request.GroupName);

            var groups = await _channelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}