﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    [AutoValidateAntiforgeryToken]
    public partial class SettingsUploadFileController : ControllerBase
    {
        private const string Route = "cms/settings/settingsUploadFile";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsUploadFileController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            ISiteRepository siteRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public string CSRFToken { get; set; }
            public bool IsSafeMode { get; set; }
            public string FileUploadDirectoryName { get; set; }
            public DateFormatType FileUploadDateFormatString { get; set; }
            public bool IsFileUploadChangeFileName { get; set; }
            public string FileUploadExtensions { get; set; }
            public long FileUploadTypeMaxSize { get; set; }
            public string FileDownloadExtensions { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string FileUploadDirectoryName { get; set; }
            public DateFormatType FileUploadDateFormatString { get; set; }
            public bool IsFileUploadChangeFileName { get; set; }
            public string FileUploadExtensions { get; set; }
            public long FileUploadTypeMaxSize { get; set; }
            public string FileDownloadExtensions { get; set; }
        }
    }
}
