﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesSaveController : ControllerBase
    {
        private const string Route = "settings/sitesSave";
        private const string RouteSettings = "settings/sitesSave/actions/settings";
        private const string RouteFiles = "settings/sitesSave/actions/files";
        private const string RouteActionsData = "settings/sitesSave/actions/data";
        private const string RouteZip = "settings/sitesSave/actions/zip";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public SitesSaveController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public class GetResult
        {
            public Site Site { get; set; }
            public string TemplateDir { get; set; }
        }

        public class SaveSettingsResult
        {
            public List<string> Directories { get; set; }
            public List<string> Files { get; set; }
        }

        public class SaveFilesResult
        {
            public Channel Channel { get; set; }
        }

        public class SaveRequest : SiteRequest
        {
            public string TemplateName { get; set; }
            public string TemplateDir { get; set; }
            public string WebSiteUrl { get; set; }
            public string Description { get; set; }
            public bool IsAllFiles { get; set; }
            public IList<string> CheckedDirectories { get; set; }
            public IList<string> CheckedFiles { get; set; }
            public bool IsSaveContents { get; set; }
            public bool IsSaveAllChannels { get; set; }
            public IList<int> CheckedChannelIds { get; set; }
        }

        public class ZipRequest
        {
            public SiteType SiteType { get; set; }
            public string DirectoryName { get; set; }
        }
    }
}
