﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Editor
{
    public partial class ActionsController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteActionsUploadVideo)]
        public async Task<ActionResult<UploadVideoResult>> UploadVideo([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return new UploadVideoResult
                {
                    Error = Constants.ErrorUpload
                };
            }

            var original = Path.GetFileName(file.FileName);
            var fileName = _pathManager.GetUploadFileName(site, original);

            if (!_pathManager.IsVideoExtensionAllowed(site, PathUtils.GetExtension(fileName)))
            {
                return new UploadVideoResult
                {
                    Error = Constants.ErrorVideoExtensionAllowed
                };
            }
            if (!_pathManager.IsVideoSizeAllowed(site, file.Length))
            {
                return new UploadVideoResult
                {
                    Error = Constants.ErrorVideoSizeAllowed
                };
            }

            var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.Video);
            var filePath = PathUtils.Combine(localDirectoryPath, fileName);

            await _pathManager.UploadAsync(file, filePath);

            var videoUrl = _pathManager.GetSiteUrlByPhysicalPath(site, filePath, true);
            var coverUrl = string.Empty;

            var vodSettings = await _vodManager.GetVodSettingsAsync();
            if (vodSettings.IsVod)
            {
                var vodPlay = await _vodManager.UploadVodAsync(filePath);
                if (vodPlay.Success)
                {
                    videoUrl = vodPlay.PlayUrl;
                    coverUrl = vodPlay.CoverUrl;
                }
            }
            else
            {
                var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Videos);
                if (isAutoStorage)
                {
                    var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                    if (success)
                    {
                        videoUrl = url;
                    }
                }
            }

            // var fileUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);
            // var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Videos);
            // if (isAutoStorage)
            // {
            //     var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
            //     if (success)
            //     {
            //         fileUrl = url;
            //     }
            // }

            return new UploadVideoResult
            {
                State = "SUCCESS",
                Url = videoUrl,
                Title = original,
                Original = original,
                Error = null
            };
        }
    }
}
