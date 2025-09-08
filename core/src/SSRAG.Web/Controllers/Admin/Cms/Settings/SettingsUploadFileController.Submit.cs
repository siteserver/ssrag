﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Configuration;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadFileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsUploadFile))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.FileUploadDirectoryName = request.FileUploadDirectoryName;
            site.FileUploadDateFormatString = request.FileUploadDateFormatString;
            site.IsFileUploadChangeFileName = request.IsFileUploadChangeFileName;
            site.FileUploadExtensions = request.FileUploadExtensions.Replace("|", ",");
            site.FileUploadTypeMaxSize = request.FileUploadTypeMaxSize * 1024;
            site.FileDownloadExtensions = request.FileDownloadExtensions.Replace("|", ",");

            if (_settingsManager.IsSafeMode)
            {
                site.FileUploadExtensions = Constants.DefaultFileUploadExtensions;
                site.FileDownloadExtensions = Constants.DefaultFileDownloadExtensions;
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