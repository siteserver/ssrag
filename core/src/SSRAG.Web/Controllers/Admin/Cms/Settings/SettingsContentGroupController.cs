﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsContentGroupController : ControllerBase
    {
        private const string Route = "cms/settings/settingsContentGroup";
        private const string RouteUpdate = "cms/settings/settingsContentGroup/actions/update";
        private const string RouteDelete = "cms/settings/settingsContentGroup/actions/delete";
        private const string RouteOrder = "cms/settings/settingsContentGroup/actions/order";

        private readonly IAuthManager _authManager;
        private readonly IContentGroupRepository _contentGroupRepository;

        public SettingsContentGroupController(
            IAuthManager authManager,
            IContentGroupRepository contentGroupRepository
        )
        {
            _authManager = authManager;
            _contentGroupRepository = contentGroupRepository;
        }

        public class GetResult
        {
            public IEnumerable<ContentGroup> Groups { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string GroupName { get; set; }
        }

        public class OrderRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public bool IsUp { get; set; }
            public int Rows { get; set; }
        }
    }
}
