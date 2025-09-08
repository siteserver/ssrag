﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesLayerSelectController : ControllerBase
    {
        public const string Route = "settings/sitesLayerSelect";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public SitesLayerSelectController(IPathManager pathManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public List<Site> Sites { get; set; }
        }
    }
}
