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
    public partial class SettingsUploadImageController : ControllerBase
    {
        private const string Route = "cms/settings/settingsUploadImage";
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsUploadImageController(
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
            public string ImageUploadDirectoryName { get; set; }
            public DateFormatType ImageUploadDateFormatString { get; set; }
            public bool IsImageUploadChangeFileName { get; set; }
            public string ImageUploadExtensions { get; set; }
            public long ImageUploadTypeMaxSize { get; set; }
            public bool IsImageAutoResize { get; set; }
            public int ImageAutoResizeWidth { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string ImageUploadDirectoryName { get; set; }
            public DateFormatType ImageUploadDateFormatString { get; set; }
            public bool IsImageUploadChangeFileName { get; set; }
            public string ImageUploadExtensions { get; set; }
            public long ImageUploadTypeMaxSize { get; set; }
            public bool IsImageAutoResize { get; set; }
            public int ImageAutoResizeWidth { get; set; }
        }
    }
}
