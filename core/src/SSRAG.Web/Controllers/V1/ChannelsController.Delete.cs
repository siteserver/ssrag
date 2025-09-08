﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [OpenApiOperation("删除栏目 API", "删除栏目，使用DELETE发起请求，请求地址为/api/v1/channels/{siteId}/{channelId}")]
        [HttpPost, Route(RouteChannelDelete)]
        public async Task<ActionResult<Channel>> Delete([FromRoute] int siteId, [FromRoute] int channelId)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null)
            {
                return this.Error(Constants.ErrorNotFound);
            }

            if (!await _authManager.HasSitePermissionsAsync(siteId, MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null)
            {
                return this.Error(Constants.ErrorNotFound);
            }

            var adminName = _authManager.AdminName;
            await _contentRepository.TrashContentsAsync(site, channelId, adminName);
            await _channelRepository.DeleteAsync(site, channelId);

            return channel;
        }
    }
}
