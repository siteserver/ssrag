﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Home.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class LayerImageUploadController : ControllerBase
    {
        private const string Route = "common/form/layerImageUpload";
        private const string RouteUpload = "common/form/layerImageUpload/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IStorageManager _storageManager;
        private readonly ISiteRepository _siteRepository;

        public LayerImageUploadController(IAuthManager authManager, IPathManager pathManager, IStorageManager storageManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _storageManager = storageManager;
            _siteRepository = siteRepository;
        }

        public class Options
        {
            public bool IsEditor { get; set; }
            public bool IsThumb { get; set; }
            public int ThumbWidth { get; set; }
            public int ThumbHeight { get; set; }
            public bool IsLinkToOriginal { get; set; }
        }

        public class SubmitRequest : Options
        {
            public int SiteId { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class SubmitResult
        {
            public string ImageUrl { get; set; }
            public string ImageVirtualUrl { get; set; }
            public string PreviewUrl { get; set; }
            public string PreviewVirtualUrl { get; set; }
        }
    }
}
