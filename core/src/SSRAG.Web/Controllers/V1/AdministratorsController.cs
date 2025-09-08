﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.V1
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class AdministratorsController : ControllerBase
    {
        private const string Route = "administrators";
        private const string RouteActionsLogin = "administrators/actions/login";
        private const string RouteActionsLogout = "administrators/actions/logout";
        private const string RouteActionsResetPassword = "administrators/actions/resetPassword";
        private const string RouteAdministrator = "administrators/{id:int}";
        private const string RouteAdministratorUpdate = "administrators/{id:int}/actions/update";
        private const string RouteAdministratorDelete = "administrators/{id:int}/actions/delete";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStatRepository _statRepository;

        public AdministratorsController(ISettingsManager settingsManager, IAuthManager authManager, IConfigRepository configRepository, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, IDbCacheRepository dbCacheRepository, ILogRepository logRepository, IStatRepository statRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
            _dbCacheRepository = dbCacheRepository;
            _logRepository = logRepository;
            _statRepository = statRepository;
        }

        public class ListRequest
        {
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class ListResult
        {
            public int Count { get; set; }
            public List<Administrator> Administrators { get; set; }
        }

        public class LoginRequest
        {
            /// <summary>
            /// 账号
            /// </summary>
            public string Account { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// 下次自动登录
            /// </summary>
            public bool IsAutoLogin { get; set; }
        }

        public class LoginResult
        {
            public Administrator Administrator { get; set; }
            public string AccessToken { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public string SessionId { get; set; }
            public bool IsEnforcePasswordChange { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
