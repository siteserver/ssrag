﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageController : ControllerBase
    {
        private const string Route = "common/editor/layerImage";
        private const string RouteUpload = "common/editor/layerImage/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly IStorageManager _storageManager;
        private readonly ISiteRepository _siteRepository;

        public LayerImageController(
            IPathManager pathManager,
            IStorageManager storageManager,
            ISiteRepository siteRepository
        )
        {
            _pathManager = pathManager;
            _storageManager = storageManager;
            _siteRepository = siteRepository;
        }

        public class Options
        {
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
            public string Url { get; set; }
        }

        public class SubmitResult
        {
            public string ImageUrl { get; set; }
            public string PreviewUrl { get; set; }
        }
    }
}
