﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Configuration;
using SSRAG.Utils;
using System.Collections.Generic;
using SSRAG.Models;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsLayerTranslateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
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
                FileUtils.DeleteFileIfExists(jsonFilePath);
            }
            summaries.Reverse();

            foreach (var summary in summaries)
            {
                await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, summary.ChannelId, summary.Id, request.TransSiteId, request.TransChannelId, TranslateType.Cut, _createManager, _authManager.AdminName);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, "转移内容", string.Empty);

            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            await _createManager.TriggerContentChangedEventAsync(request.TransSiteId, request.TransChannelId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}