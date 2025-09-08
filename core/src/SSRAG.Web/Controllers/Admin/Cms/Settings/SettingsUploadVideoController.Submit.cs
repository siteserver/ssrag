﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Configuration;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadVideoController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsUploadVideo))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.VideoUploadDirectoryName = request.VideoUploadDirectoryName;
            site.VideoUploadDateFormatString = request.VideoUploadDateFormatString;
            site.IsVideoUploadChangeFileName = request.IsVideoUploadChangeFileName;
            site.VideoUploadExtensions = request.VideoUploadExtensions.Replace("|", ",");
            site.VideoUploadTypeMaxSize = request.VideoUploadTypeMaxSize * 1024;

            if (_settingsManager.IsSafeMode)
            {
                site.VideoUploadExtensions = Constants.DefaultVideoUploadExtensions;
            }

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改视频上传设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}