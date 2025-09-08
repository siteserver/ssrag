﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersLayerProfileController : ControllerBase
    {
        private const string Route = "settings/usersLayerProfile";
        private const string RouteUpload = "settings/usersLayerProfile/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICloudManager _cloudManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public UsersLayerProfileController(IAuthManager authManager, IPathManager pathManager, ICloudManager cloudManager, IUserRepository userRepository, IUserGroupRepository userGroupRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _cloudManager = cloudManager;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public User User { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
        }

        public class UploadRequest
        {
            public int UserId { get; set; }
            public IFormFile File { set; get; }
        }
    }
}
