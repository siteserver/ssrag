﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Common
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdminLayerViewController : ControllerBase
    {
        private const string Route = "common/adminLayerView";

        private readonly IHttpContextAccessor _context;
        private readonly IAntiforgery _antiforgery;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;

        public AdminLayerViewController(
            IHttpContextAccessor context,
            IAntiforgery antiforgery,
            ISettingsManager settingsManager,
            IDatabaseManager databaseManager,
            IAdministratorRepository administratorRepository,
            ISiteRepository siteRepository
        )
        {
            _context = context;
            _antiforgery = antiforgery;
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
        }

        public class GetRequest
        {
            public string Uuid { get; set; }
        }

        public class GetResult
        {
            public Administrator Administrator { get; set; }
            public string Level { get; set; }
            public string SiteNames { get; set; }
            public string RoleNames { get; set; }
        }
    }
}
