﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Configuration;
using SSRAG.Utils;
using SSRAG.Models;
using System.Collections.Generic;

namespace SSRAG.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerDeleteController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Contents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var originalSummaries = new List<ChannelContentId>();
            if (!string.IsNullOrEmpty(request.FileName))
            {
                var jsonFilePath = _pathManager.GetTemporaryFilesPath(request.FileName);
                if (FileUtils.IsFileExists(jsonFilePath))
                {
                    var json = await FileUtils.ReadTextAsync(jsonFilePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        originalSummaries = TranslateUtils.JsonDeserialize<List<ChannelContentId>>(json);
                    }
                    FileUtils.DeleteFileIfExists(jsonFilePath);
                }
            }

            var summaries = new List<ChannelContentId>();
            foreach (var summary in originalSummaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                if (!await _authManager.HasContentPermissionsAsync(request.SiteId, channel.Id, MenuUtils.ContentPermissions.Delete))
                {
                    return Unauthorized();
                }

                if (!channel.IsChangeBanned)
                {
                    summaries.Add(summary);
                }
            }

            if (!request.IsRetainFiles)
            {
                foreach (var summary in summaries)
                {
                    await _createManager.DeleteContentAsync(site, summary.ChannelId, summary.Id);
                }
            }

            if (summaries.Count == 1)
            {
                var summary = summaries[0];

                var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                if (content != null)
                {
                    await _authManager.AddSiteLogAsync(request.SiteId, summary.ChannelId, summary.Id, "删除内容",
                        $"栏目：{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, summary.ChannelId)}，内容标题：{content.Title}");
                }
            }
            else
            {
                await _authManager.AddSiteLogAsync(request.SiteId, "批量删除内容", $"内容条数：{summaries.Count}");
            }

            var adminName = _authManager.AdminName;
            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                var distinctChannel = await _channelRepository.GetAsync(distinctChannelId);
                var contentIdList = summaries.Where(x => x.ChannelId == distinctChannelId)
                    .Select(x => x.Id).ToList();
                await _contentRepository.TrashContentsAsync(site, distinctChannel, contentIdList, adminName);

                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}