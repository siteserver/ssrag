﻿using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsIfController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUsersInGroupsRepository _usersInGroupsRepository;

        public ActionsIfController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IParseManager parseManager,
            IUserGroupRepository userGroupRepository,
            IUsersInGroupsRepository usersInGroupsRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _userGroupRepository = userGroupRepository;
            _usersInGroupsRepository = usersInGroupsRepository;
        }

        public class GetRequest
        {
            public string Value { get; set; }
            public int Page { get; set; }
        }

        public class GetResult
        {
            public bool Value { get; set; }
            public string Html { get; set; }
        }
    }
}
