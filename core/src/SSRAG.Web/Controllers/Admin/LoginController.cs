﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LoginController : ControllerBase
    {
        public const string Route = "login";
        private const string RouteCaptcha = "login/captcha";
        private const string RouteSendSms = "login/actions/sendSms";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISmsManager _smsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStatRepository _statRepository;

        public LoginController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ISmsManager smsManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, IDbCacheRepository dbCacheRepository, ILogRepository logRepository, IStatRepository statRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _smsManager = smsManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _dbCacheRepository = dbCacheRepository;
            _logRepository = logRepository;
            _statRepository = statRepository;
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string RedirectUrl { get; set; }
            public string Version { get; set; }
            public string AdminFaviconUrl { get; set; }
            public string AdminTitle { get; set; }
            public bool IsAdminCaptchaDisabled { get; set; }
            public bool IsSmsAdmin { get; set; }
            public bool IsSmsAdminAndDisableAccount { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsSmsLogin { get; set; }
            public string Account { get; set; }
            public string Password { get; set; }
            public string Mobile { get; set; }
            public string Code { get; set; }
            public bool IsPersistent { get; set; }
            public bool IsForceLogoutAndLogin { get; set; }
            public string Token { get; set; }
            public string Value { get; set; }
        }

        public class SubmitResult
        {
            public bool IsLoginExists { get; set; }
            public Administrator Administrator { get; set; }
            public string SessionId { get; set; }
            public bool IsEnforcePasswordChange { get; set; }
            public string Token { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return _settingsManager.Cache.GetClassKey(typeof(LoginController), nameof(Administrator), mobile);
        }

        private async Task<string> AdminRedirectCheckAsync()
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            var config = await _configRepository.GetAsync();

            if (string.IsNullOrEmpty(_settingsManager.Database.ConnectionString))
            {
                redirect = true;
                redirectUrl = _pathManager.GetAdminUrl(InstallController.Route);
            }
            else if (config.Initialized &&
                     config.DatabaseVersion != _settingsManager.Version)
            {
                redirect = true;
                redirectUrl = _pathManager.GetAdminUrl(SyncDatabaseController.Route);
            }

            return redirect ? redirectUrl : null;
        }
    }
}