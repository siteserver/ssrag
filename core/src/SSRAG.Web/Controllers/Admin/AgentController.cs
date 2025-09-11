﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AgentController : ControllerBase
    {
        public const string Route = "agent";
        private const string RouteSites = "agent/sites";
        private const string RoutePlugins = "agent/plugins";
        private const string RouteInstall = "agent/actions/install";
        private const string RouteSetDomain = "agent/actions/setDomain";
        private const string RouteAddSite = "agent/actions/addSite";
        private const string RouteProcess = "agent/actions/process";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ICreateManager _createManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IContentRepository _contentRepository;

        public AgentController(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ICreateManager createManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, IDbCacheRepository dbCacheRepository, IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _createManager = createManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _dbCacheRepository = dbCacheRepository;
            _contentRepository = contentRepository;
        }

        public class AgentRequest
        {
            public string SecurityKey { get; set; }
        }

        public class InstallRequest : AgentRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public class SitesResult
        {
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
        }

        public class SetDomainRequest : AgentRequest
        {
            public string HostDomain { get; set; }
            public int SiteId { get; set; }
            public string SiteDomain { get; set; }
        }

        public class AddSiteRequest : AgentRequest
        {
            public SiteType SiteType { get; set; }
            public string SiteName { get; set; }
            public bool Root { get; set; }
            public string SiteDir { get; set; }
            public string ThemeDownloadUrl { get; set; }
            public string Uuid { get; set; }
        }

        public class AddSiteResult
        {
            public Site Site { get; set; }
        }

        public class ProcessRequest : AgentRequest
        {
            public string Uuid { get; set; }
        }

        public class AgentPlugin
        {
            public string Publisher { get; set; }
            public string Name { get; set; }
            public string Version { get; set; }
        }

        public class PluginsResult
        {
            public List<AgentPlugin> Plugins { get; set; }
        }
    }
}