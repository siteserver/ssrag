﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerCheckController : ControllerBase
    {
        private const string Route = "document/contents/contentsLayerCheck";
        private const string RouteOptions = "document/contents/contentsLayerCheck/actions/options";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentCheckRepository _contentCheckRepository;

        public ContentsLayerCheckController(
            IAuthManager authManager,
            IPathManager pathManager,
            ICreateManager createManager,
            IDatabaseManager databaseManager,
            IPluginManager pluginManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository,
            IContentCheckRepository contentCheckRepository
        )
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentCheckRepository = contentCheckRepository;
        }

        public class GetRequest : SiteRequest
        {
            public string ChannelContentIds { get; set; }
            public string FileName { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<Content> Contents { get; set; }
            public IEnumerable<Select<int>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
            public IEnumerable<Select<int>> TransSites { get; set; }
        }

        public class GetOptionsRequest : SiteRequest
        {
            public int TransSiteId { get; set; }
        }

        public class GetOptionsResult
        {
            public Cascade<int> TransChannels { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string ChannelContentIds { get; set; }
            public string FileName { get; set; }
            public int CheckedLevel { get; set; }
            public string Reasons { get; set; }
            public bool IsTranslate { get; set; }
            public int TransSiteId { get; set; }
            public int TransChannelId { get; set; }

        }
    }
}
