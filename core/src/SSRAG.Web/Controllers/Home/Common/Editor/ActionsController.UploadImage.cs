﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Common.Editor
{
    public partial class ActionsController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteActionsUploadImage)]
        public async Task<ActionResult<UploadImageResult>> UploadImage([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return new UploadImageResult
                {
                    Error = Constants.ErrorUpload
                };
            }

            var original = Path.GetFileName(file.FileName);
            var (success, filePath, errorMessage) = await _pathManager.UploadImageAsync(site, file);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var imageUrl = _pathManager.GetSiteUrlByPhysicalPath(site, filePath, true);

            return new UploadImageResult
            {
                State = "SUCCESS",
                Url = imageUrl,
                Title = original,
                Original = original,
                Error = null
            };
        }

        public class UploadImageResult
        {
            public string State { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string Original { get; set; }
            public string Error { get; set; }
        }
    }
}
