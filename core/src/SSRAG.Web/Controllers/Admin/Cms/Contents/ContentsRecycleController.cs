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

namespace SSRAG.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsRecycleController : ControllerBase
    {
        private const string Route = "cms/contents/contentsRecycle";
        private const string RouteDelete = "cms/contents/contentsRecycle/actions/delete";
        private const string RouteList = "cms/contents/contentsRecycle/actions/list";
        private const string RouteTree = "cms/contents/contentsRecycle/actions/tree";
        private const string RouteColumns = "cms/contents/contentsRecycle/actions/columns";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;

        public ContentsRecycleController(
            IAuthManager authManager,
            IPathManager pathManager,
            IDatabaseManager databaseManager,
            IPluginManager pluginManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository,
            IContentGroupRepository contentGroupRepository,
            IContentTagRepository contentTagRepository
        )
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
        }

        public enum Action
        {
            Delete,
            DeleteAll,
            Restore,
            RestoreAll
        }

        public class DeleteRequest : SiteRequest
        {
            public Action Action { get; set; }
            public string ChannelContentIds { get; set; }
            public int RestoreChannelId { get; set; }
        }

        public class TreeResult
        {
            public Cascade<int> Root { get; set; }
            public int ChannelId { get; set; }
            public string SiteUrl { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public ContentColumn TitleColumn { get; set; }
            public ContentColumn BodyColumn { get; set; }
        }

        public class ListRequest : SiteRequest
        {
            public int? ChannelId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public IEnumerable<KeyValuePair<string, string>> Items { get; set; }
            public int Page { get; set; }
            public bool IsCheckedLevels { get; set; }
            public List<int> CheckedLevels { get; set; }
            public bool IsTop { get; set; }
            public bool IsRecommend { get; set; }
            public bool IsHot { get; set; }
            public bool IsColor { get; set; }
            public List<string> GroupNames { get; set; }
            public List<string> TagNames { get; set; }
        }

        public class ListResult
        {
            public List<Content> PageContents { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
        }
    }
}