﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerFileUploadController : ControllerBase
    {
        private const string Route = "common/form/layerFileUpload";
        private const string RouteUpload = "common/form/layerFileUpload/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly IStorageManager _storageManager;
        private readonly ISiteRepository _siteRepository;

        public LayerFileUploadController(
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
            public bool IsChangeFileName { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public bool IsChangeFileName { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class SubmitRequest : Options
        {
            public int SiteId { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class SubmitResult
        {
            public string FileUrl { get; set; }
            public string FileVirtualUrl { get; set; }
        }
    }
}
