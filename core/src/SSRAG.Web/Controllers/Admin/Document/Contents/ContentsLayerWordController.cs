﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerWordController : ControllerBase
    {
        private const string Route = "document/contents/contentsLayerWord";
        private const string RouteUpload = "document/contents/contentsLayerWord/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public ContentsLayerWordController(
            IAuthManager authManager,
            IPathManager pathManager,
            ICreateManager createManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository,
            IErrorLogRepository errorLogRepository
        )
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _errorLogRepository = errorLogRepository;
        }

        public class GetResult
        {
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
        }

        public class UploadResult
        {
            public string FileName { get; set; }
            public string FileUrl { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public bool IsFirstLineTitle { get; set; }
            public bool IsClearFormat { get; set; }
            public bool IsFirstLineIndent { get; set; }
            public bool IsClearFontSize { get; set; }
            public bool IsClearFontFamily { get; set; }
            public bool IsClearImages { get; set; }
            public int CheckedLevel { get; set; }
            public List<string> FileNames { get; set; }
            public List<string> FileUrls { get; set; }
        }

        public class SubmitResult
        {
            public List<int> ContentIds { get; set; }
            public bool IsIndex { get; set; }
        }
    }
}
