﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Configuration;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerTranslateController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Translate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            // var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            var summaries = new List<ChannelContentId>();
            var jsonFilePath = _pathManager.GetTemporaryFilesPath(request.FileName);
            if (FileUtils.IsFileExists(jsonFilePath))
            {
                var json = await FileUtils.ReadTextAsync(jsonFilePath);
                if (!string.IsNullOrEmpty(json))
                {
                    summaries = TranslateUtils.JsonDeserialize<List<ChannelContentId>>(json);
                }
            }

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                var pageContent = content.Clone<Content>();
                pageContent.Set(ColumnsManager.CheckState, CheckManager.GetCheckState(site, content));
                contents.Add(pageContent);
            }

            var siteIdList = await _authManager.GetSiteIdsAsync();
            var transSites = await _siteRepository.GetSelectsAsync(siteIdList);

            return new GetResult
            {
                Contents = contents,
                TransSites = transSites
            };
        }
    }
}