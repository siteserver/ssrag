﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Cms.Create
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class CreatePageController : ControllerBase
    {
        private const string Route = "cms/create/createPage";
        private const string RouteAll = "cms/create/createPage/all";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITemplateRepository _templateRepository;

        public CreatePageController(
            IAuthManager authManager,
            ICreateManager createManager,
            IPathManager pathManager,
            IConfigRepository configRepository,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository,
            ITemplateRepository templateRepository
        )
        {
            _authManager = authManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _templateRepository = templateRepository;
        }

        public class GetRequest : SiteRequest
        {
            public CreateType Type { get; set; }
            public int ParentId { get; set; }
        }

        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public IEnumerable<int> AllChannelIds { get; set; }
            public IEnumerable<Template> ChannelTemplates { get; set; }
            public IEnumerable<Template> ContentTemplates { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public CreateType Type { get; set; }
            public IEnumerable<int> ChannelIdList { get; set; }
            public bool IsAllChecked { get; set; }
            public bool IsDescendent { get; set; }
            public bool IsChannelPage { get; set; }
            public bool IsContentPage { get; set; }
            public string Scope { get; set; }
        }
    }
}
