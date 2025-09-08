using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Utilities
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UtilitiesCacheController : ControllerBase
    {
        private const string Route = "settings/utilitiesCache";
        private const string RouteClearCache = "settings/utilitiesCache/actions/clearCache";
        private const string RouteRestart = "settings/utilitiesCache/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IAuthManager _authManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public UtilitiesCacheController(IHostApplicationLifetime hostApplicationLifetime, ISettingsManager settingsManager, IPathManager pathManager, IAuthManager authManager, IDbCacheRepository dbCacheRepository)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _authManager = authManager;
            _dbCacheRepository = dbCacheRepository;
        }

        public class GetResult
        {
            public List<string> Keys { get; set; }
        }
    }
}
