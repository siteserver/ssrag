﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteDrop)]
        public async Task<ActionResult<BoolResult>> Drop([FromBody] DropRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            await _channelRepository.DropAsync(request.SiteId, request.SourceId, request.TargetId, request.DropType);
            await _authManager.AddSiteLogAsync(request.SiteId, "对栏目进行排序");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
