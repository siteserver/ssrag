﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsRoleAddController : ControllerBase
    {
        private const string Route = "settings/administratorsRoleAdd";
        private const string RouteUpdate = "settings/administratorsRoleAdd/actions/update";
        private const string RouteSitePermission = "settings/administratorsRoleAdd/actions/sitePermission";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;
        private readonly IFormRepository _formRepository;

        public AdministratorsRoleAddController(ISettingsManager settingsManager, IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IRoleRepository roleRepository, ISitePermissionsRepository sitePermissionsRepository, IPermissionsInRolesRepository permissionsInRolesRepository, IFormRepository formRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _roleRepository = roleRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
            _formRepository = formRepository;
        }

        public class Option
        {
            public string Name { get; set; }

            public string Text { get; set; }

            public bool Selected { get; set; }
        }

        public class GetRequest
        {
            public int RoleId { get; set; }
        }

        public class GetResult
        {
            public Role Role { get; set; }
            public List<Option> Permissions { get; set; }
            public List<Site> Sites { get; set; }
            public List<SitePermissions> SitePermissions { get; set; }
        }

        public class SubmitRequest
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }
            public List<string> AppPermissions { get; set; }
            public List<SitePermissions> SitePermissions { get; set; }
        }

        public class SitePermissionRequest
        {
            public int RoleId { get; set; }
            public int SiteId { get; set; }
        }

        public class SitePermissionResult
        {
            public Site Site { get; set; }
            public List<Option> SitePermissions { get; set; }
            public List<Option> ContentPermissions { get; set; }
            public Channel Channel { get; set; }
            public List<int> CheckedChannelIds { get; set; }
        }
    }
}
