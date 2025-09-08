﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Plugins
{
    public partial class ConfigController
    {
        [HttpPost, Route(RouteActionsGetChannels)]
        public async Task<ActionResult<GetChannelsResult>> GetChannels([FromBody] GetChannelsRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var channel = await _channelRepository.GetAsync(request.SiteId);
            channel.Children = await _channelRepository.GetChildrenAsync(request.SiteId, request.SiteId);

            var plugin = _pluginManager.GetPlugin(request.PluginId);
            SiteConfig siteConfig = null;
            if (plugin.SiteConfigs != null)
            {
                siteConfig = plugin.SiteConfigs.FirstOrDefault(x => x.SiteId == request.SiteId);
            }

            siteConfig ??= new SiteConfig
            {
                SiteId = request.SiteId,
                AllChannels = plugin.AllChannels,
                ChannelIds = new List<int>()
            };

            return new GetChannelsResult
            {
                SiteName = site.SiteName,
                Channel = channel,
                SiteConfig = siteConfig
            };
        }
    }
}