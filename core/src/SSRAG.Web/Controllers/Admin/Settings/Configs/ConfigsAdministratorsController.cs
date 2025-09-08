using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Configs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ConfigsAdministratorsController : ControllerBase
    {
        private const string Route = "settings/configsAdministrators";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public ConfigsAdministratorsController(IAuthManager authManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public Config Config { get; set; }
        }

        public class SubmitRequest
        {
            public int AdminUserNameMinLength { get; set; }
            public int AdminPasswordMinLength { get; set; }
            public PasswordRestriction AdminPasswordRestriction { get; set; }
            public bool IsAdminLockLogin { get; set; }
            public int AdminLockLoginCount { get; set; }
            public LockType AdminLockLoginType { get; set; }
            public int AdminLockLoginHours { get; set; }
            public bool IsAdminEnforcePasswordChange { get; set; }
            public int AdminEnforcePasswordChangeDays { get; set; }
            public bool IsAdminEnforceLogout { get; set; }
            public int AdminEnforceLogoutMinutes { get; set; }
            public bool IsAdminCaptchaDisabled { get; set; }
        }
    }
}
