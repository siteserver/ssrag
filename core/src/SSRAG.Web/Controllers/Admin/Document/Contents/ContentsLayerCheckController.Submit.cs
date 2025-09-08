﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Configuration;
using SSRAG.Utils;
using System.Collections.Generic;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsLayerCheckController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ContentsCheck))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                request.CheckedLevel = 0;
            }

            // var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            // var summaries = new List<ChannelContentId>();
            // var jsonFilePath = _pathManager.GetTemporaryFilesPath(request.FileName);
            // if (FileUtils.IsFileExists(jsonFilePath))
            // {
            //     var json = await FileUtils.ReadTextAsync(jsonFilePath);
            //     if (!string.IsNullOrEmpty(json))
            //     {
            //         summaries = TranslateUtils.JsonDeserialize<List<ChannelContentId>>(json);
            //     }
            //     FileUtils.DeleteFileIfExists(jsonFilePath);
            // }

            var summaries = new List<ChannelContentId>();
            if (!string.IsNullOrEmpty(request.ChannelContentIds))
            {
                summaries = ContentUtility.ParseChannelContentIds(request.ChannelContentIds);
            }
            else
            {
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
            }

            summaries.Reverse();

            var adminName = _authManager.AdminName;
            foreach (var summary in summaries)
            {
                var contentChannelInfo = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, contentChannelInfo, summary.Id);
                if (content == null) continue;

                if (!await _authManager.HasContentPermissionsAsync(request.SiteId, content.ChannelId, MenuUtils.ContentPermissions.CheckLevel1))
                {
                    return Unauthorized();
                }

                content.Set(ColumnsManager.CheckAdminName, adminName);
                content.Set(ColumnsManager.CheckDate, DateTime.Now);
                content.Set(ColumnsManager.CheckReasons, request.Reasons);

                content.Checked = isChecked;
                content.CheckedLevel = request.CheckedLevel;

                await _contentRepository.UpdateAsync(site, contentChannelInfo, content);

                await _contentCheckRepository.InsertAsync(new ContentCheck
                {
                    SiteId = request.SiteId,
                    ChannelId = content.ChannelId,
                    ContentId = content.Id,
                    AdminName = adminName,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel,
                    CheckDate = DateTime.Now,
                    Reasons = request.Reasons
                });

                if (request.IsTranslate)
                {
                    await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, summary.ChannelId, summary.Id, request.TransSiteId, request.TransChannelId, TranslateType.Cut, _createManager, _authManager.AdminName);
                }
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量审核内容");

            if (request.IsTranslate)
            {
                await _createManager.TriggerContentChangedEventAsync(request.TransSiteId, request.TransChannelId);
            }
            else
            {
                foreach (var summary in summaries)
                {
                    await _createManager.CreateContentAsync(request.SiteId, summary.ChannelId, summary.Id);
                }
            }

            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}