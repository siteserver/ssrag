﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerExportController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerExport";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ContentsLayerExportController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IPathManager pathManager,
            IDatabaseManager databaseManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository,
            ITableStyleRepository tableStyleRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public List<ContentColumn> Value { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string FileName { get; set; }
            public string ExportType { get; set; }
            public bool IsAllCheckedLevel { get; set; }
            public List<int> CheckedLevelKeys { get; set; }
            public bool IsAllDate { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool IsAllColumns { get; set; }
            public List<string> ColumnNames { get; set; }
        }

        public class SubmitResult
        {
            public string Value { get; set; }
            public bool IsSuccess { get; set; }
        }
    }
}
