﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Configuration;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadFileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsUploadFile))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var fileUploadExtensions = site.FileUploadExtensions;
            var fileDownloadExtensions = site.FileDownloadExtensions;
            var fileUploadTypeMaxSize = site.FileUploadTypeMaxSize / 1024;
            
            if (_settingsManager.IsSafeMode)
            {
                fileUploadExtensions = Constants.DefaultFileUploadExtensions;
                fileDownloadExtensions = Constants.DefaultFileDownloadExtensions;
            }

            return new GetResult
            {
                CSRFToken = _authManager.GetCSRFToken(),
                IsSafeMode = _settingsManager.IsSafeMode,
                FileUploadDirectoryName = site.FileUploadDirectoryName,
                FileUploadDateFormatString = site.FileUploadDateFormatString,
                IsFileUploadChangeFileName = site.IsFileUploadChangeFileName,
                FileUploadExtensions = fileUploadExtensions,
                FileUploadTypeMaxSize = fileUploadTypeMaxSize,
                FileDownloadExtensions = fileDownloadExtensions,
            };
        }
    }
}