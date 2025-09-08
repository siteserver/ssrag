using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils.Office;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Configuration;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsLayerWordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;

            var adminName = _authManager.AdminName;
            var contentIdList = new List<int>();

            for (var i = 0; i < request.FileNames.Count; i++)
            {
                var fileName = request.FileNames[i];
                var fileUrl = request.FileUrls[i];
                var title = PathUtils.GetFileNameWithoutExtension(fileName);

                if (string.IsNullOrEmpty(fileName)) continue;

                try
                {
                    var filePath = _pathManager.GetTemporaryFilesPath(fileUrl);
                    var wordManager = new WordManager(request.IsFirstLineTitle, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath, title);
                    await wordManager.ParseAsync(_pathManager, site);

                    if (string.IsNullOrEmpty(wordManager.Title)) continue;

                    var contentInfo = new Content
                    {
                        ChannelId = channel.Id,
                        SiteId = request.SiteId,
                        AdminName = adminName,
                        LastEditAdminName = adminName,
                        AddDate = DateTime.Now,
                        Checked = isChecked,
                        CheckedLevel = request.CheckedLevel,
                        Title = wordManager.Title,
                        ImageUrl = wordManager.ImageUrl,
                        Body = wordManager.Body
                    };

                    await _contentRepository.InsertAsync(site, channel, contentInfo);
                    contentIdList.Add(contentInfo.Id);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(ex);
                }
            }

            if (isChecked)
            {
                foreach (var contentId in contentIdList)
                {
                    await _createManager.CreateContentAsync(request.SiteId, channel.Id, contentId);
                }
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, channel.Id);
            }

            return new SubmitResult
            {
                ContentIds = contentIdList,
                IsIndex = channel.Knowledge && isChecked
            };
        }
    }
}