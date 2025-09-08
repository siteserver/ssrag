﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsContentTagController : ControllerBase
    {
        private const string Route = "cms/settings/settingsContentTag";
        private const string RouteDelete = "cms/settings/settingsContentTag/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentTagRepository _contentTagRepository;

        public SettingsContentTagController(
            IAuthManager authManager,
            ISiteRepository siteRepository,
            IContentTagRepository contentTagRepository
        )
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _contentTagRepository = contentTagRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class GetResult
        {
            public int Total { get; set; }
            public IEnumerable<string> TagNames { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string TagNames { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string TagName { get; set; }
        }
    }
}