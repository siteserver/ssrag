﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var groupNames = await _channelRepository.GetGroupNamesAsync(summary.Id);

                var node = await _channelRepository.GetAsync(summary.Id);
                var imageUrl = string.Empty;
                if (node != null && !string.IsNullOrEmpty(node.ImageUrl))
                {
                    imageUrl = _pathManager.ParseSiteUrl(site, node.ImageUrl, true);
                }

                return new
                {
                    Channel = node,
                    Count = count,
                    ImageUrl = imageUrl,
                    GroupNames = groupNames,
                };
            });

            var indexNames = await _channelRepository.GetChannelIndexNamesAsync(request.SiteId);
            var groupNameList = await _channelGroupRepository.GetGroupNamesAsync(request.SiteId);

            var channelTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            var max = 0;
            var channelPlugins = _pluginManager.GetPlugins(request.SiteId);
            var channelMenus = new List<Menu>();
            var channelsMenus = new List<Menu>();
            foreach (var plugin in channelPlugins)
            {
                var pluginMenus = plugin.GetMenus()
                    .Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.MenuTypes.Channel)).ToList();
                if (pluginMenus.Count > 0)
                {
                    channelMenus.AddRange(pluginMenus);
                }
                pluginMenus = plugin.GetMenus()
                    .Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.MenuTypes.Channels)).ToList();
                if (pluginMenus.Count > 0)
                {
                    channelsMenus.AddRange(pluginMenus);
                }
            }

            max = Math.Max(max, channelMenus.Count);

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetChannelListColumnsAsync(site);

            var commandsWidth = 160 + 40 * max;
            var isTemplateEditable =
                await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates);

            var linkTypes = _pathManager.GetLinkTypeSelects(true);

            var taxisTypes = new List<Select<string>>
            {
                new Select<string>(TaxisType.OrderByTaxisDesc),
                new Select<string>(TaxisType.OrderByTaxis),
                new Select<string>(TaxisType.OrderByAddDateDesc),
                new Select<string>(TaxisType.OrderByAddDate)
            };
            var siteUrl = _pathManager.GetSiteUrl(site, true);

            return new ListResult
            {
                Channel = cascade,
                IndexNames = indexNames,
                GroupNames = groupNameList,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates,
                Columns = columns,
                CommandsWidth = commandsWidth,
                IsTemplateEditable = isTemplateEditable,
                LinkTypes = linkTypes,
                TaxisTypes = taxisTypes,
                SiteUrl = siteUrl,
                ChannelMenus = channelMenus,
                ChannelsMenus = channelsMenus,
            };
        }
    }
}
