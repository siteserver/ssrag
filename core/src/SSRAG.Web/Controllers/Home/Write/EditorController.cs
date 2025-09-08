﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Home.Write
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class EditorController : ControllerBase
    {
        private const string Route = "write/editor";
        private const string RouteUpdate = "write/editor/actions/update";
        private const string RoutePreview = "write/editor/actions/preview";

        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly IStorageManager _storageManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly IContentCheckRepository _contentCheckRepository;

        public EditorController(IAuthManager authManager, ICloudManager cloudManager, ICreateManager createManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, IStorageManager storageManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository, ITableStyleRepository tableStyleRepository, IRelatedFieldItemRepository relatedFieldItemRepository, IContentCheckRepository contentCheckRepository)
        {
            _authManager = authManager;
            _cloudManager = cloudManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _storageManager = storageManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
            _tableStyleRepository = tableStyleRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
            _contentCheckRepository = contentCheckRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public bool Unauthorized { get; set; }
            public Content Content { get; set; }
            public Site Site { get; set; }
            public Channel Channel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
            public Dictionary<int, List<Cascade<int>>> RelatedFields { get; set; }
            public List<Select<int>> CheckedLevels { get; set; }
            public string SiteUrl { get; set; }
        }

        public class PreviewRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
        }

        public class PreviewResult
        {
            public string Url { get; set; }
        }

        public class SaveRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
        }
    }
}
