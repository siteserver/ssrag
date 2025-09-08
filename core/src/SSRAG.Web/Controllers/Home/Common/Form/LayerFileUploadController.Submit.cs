﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Common.Form
{
    public partial class LayerFileUploadController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<SubmitResult>>> Submit([FromBody] SubmitRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Files);

            var result = new List<SubmitResult>();
            foreach (var filePath in request.FilePaths)
            {
                if (string.IsNullOrEmpty(filePath)) continue;

                var virtualUrl = _pathManager.GetVirtualUrlByPhysicalPath(site, filePath);
                var fileUrl = _pathManager.ParseSiteUrl(site, virtualUrl, true);
                if (isAutoStorage)
                {
                    var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                    if (success)
                    {
                        virtualUrl = fileUrl = url;
                    }
                }

                result.Add(new SubmitResult
                {
                    FileUrl = fileUrl,
                    FileVirtualUrl = virtualUrl
                });
            }

            var options = TranslateUtils.JsonDeserialize(site.Get<string>($"Home.{nameof(LayerFileUploadController)}"), new Options
            {
                IsChangeFileName = true
            });

            options.IsChangeFileName = request.IsChangeFileName;
            site.Set($"Home.{nameof(LayerFileUploadController)}", TranslateUtils.JsonSerialize(options));

            await _siteRepository.UpdateAsync(site);

            return result;
        }
    }
}
