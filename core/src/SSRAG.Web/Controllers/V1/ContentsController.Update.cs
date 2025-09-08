﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("修改内容 API", "修改内容，使用POST发起请求，请求地址为/api/v1/contents/{siteId}/{channelId}/{id}")]
        [HttpPost, Route(RouteContentUpdate)]
        public async Task<ActionResult<Content>> Update([FromRoute] int siteId, [FromRoute] int channelId, [FromRoute] int id, [FromBody] Dictionary<string, object> request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            var content = await _contentRepository.GetAsync(site, channel, id);
            if (content == null) return this.Error(Constants.ErrorNotFound);

            if (!await _authManager.HasContentPermissionsAsync(siteId, channelId, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            content.LoadDict(request);

            content.SiteId = siteId;
            content.ChannelId = channelId;
            content.LastEditAdminName = _authManager.AdminName;

            var postCheckedLevel = content.CheckedLevel;
            var isChecked = postCheckedLevel >= site.CheckContentLevel;
            var checkedLevel = postCheckedLevel;

            content.Checked = isChecked;
            content.CheckedLevel = checkedLevel;

            await _contentRepository.UpdateAsync(site, channel, content);

            //foreach (var plugin in _pluginManager.GetPlugins(siteId, channelId))
            //{
            //    try
            //    {
            //        plugin.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, content.Id, content.ToDictionary(), content));
            //    }
            //    catch (Exception ex)
            //    {
            //        await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(IOldPlugin.ContentFormSubmit));
            //    }
            //}

            if (content.Checked)
            {
                await _createManager.CreateContentAsync(siteId, channelId, content.Id);
                await _createManager.TriggerContentChangedEventAsync(siteId, channelId);
            }

            await _authManager.AddSiteLogAsync(siteId, channelId, content.Id, "修改内容",
                $"栏目：{await _channelRepository.GetChannelNameNavigationAsync(siteId, content.ChannelId)}，内容标题：{content.Title}");

            return content;
        }
    }
}
