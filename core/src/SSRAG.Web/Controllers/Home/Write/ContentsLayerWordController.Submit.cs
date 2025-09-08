﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Core.Utils.Office;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Write
{
    public partial class ContentsLayerWordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            var styles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            var adminName = _authManager.AdminName;
            var userName = _authManager.UserName;

            var contentIdList = new List<int>();
            foreach (var file in request.Files)
            {
                if (string.IsNullOrEmpty(file.FileName) || string.IsNullOrEmpty(file.Title)) continue;

                var filePath = _pathManager.GetTemporaryFilesPath(file.FileName);
                var wordManager = new WordManager(request.IsFirstLineTitle, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath, file.Title);
                await wordManager.ParseAsync(_pathManager, site);

                if (string.IsNullOrEmpty(wordManager.Title)) continue;

                var dict = await ColumnsManager.SaveAttributesAsync(_pathManager, site, styles, new NameValueCollection(), ColumnsManager.MetadataAttributes.Value);

                var contentInfo = new Content
                {
                    ChannelId = channel.Id,
                    SiteId = request.SiteId,
                    AddDate = DateTime.Now,
                    SourceId = SourceManager.User,
                    AdminName = adminName,
                    UserName = userName,
                    LastEditAdminName = adminName,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel
                };
                contentInfo.LoadDict(dict);

                contentInfo.Title = wordManager.Title;
                contentInfo.ImageUrl = wordManager.ImageUrl;
                contentInfo.Body = wordManager.Body;

                contentInfo.Id = await _contentRepository.InsertAsync(site, channel, contentInfo);

                contentIdList.Add(contentInfo.Id);
            }

            if (isChecked)
            {
                foreach (var contentId in contentIdList)
                {
                    await _createManager.CreateContentAsync(request.SiteId, channel.Id, contentId);
                }
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, channel.Id);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
