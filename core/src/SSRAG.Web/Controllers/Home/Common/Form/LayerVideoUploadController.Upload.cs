﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Common.Form
{
    public partial class LayerVideoUploadController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!_pathManager.IsVideoExtensionAllowed(site, PathUtils.GetExtension(fileName)))
            {
                return this.Error(Constants.ErrorVideoExtensionAllowed);
            }
            if (!_pathManager.IsVideoSizeAllowed(site, file.Length))
            {
                return this.Error(Constants.ErrorVideoSizeAllowed);
            }

            var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.Video);
            var localFileName = PathUtils.GetUploadFileName(fileName, request.IsChangeFileName);
            var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

            await _pathManager.UploadAsync(file, filePath);

            return new UploadResult
            {
                Name = localFileName,
                Path = filePath
            };
        }
    }
}
