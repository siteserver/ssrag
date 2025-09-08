using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class LoginController : ControllerBase
    {
        private const string Route = "login";
        private const string RouteCaptcha = "login/captcha";
        private const string RouteSendSms = "login/actions/sendSms";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ISmsManager _smsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStatRepository _statRepository;

        public LoginController(ISettingsManager settingsManager, IAuthManager authManager, ISmsManager smsManager, IConfigRepository configRepository, IUserRepository userRepository, ILogRepository logRepository, IStatRepository statRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _smsManager = smsManager;
            _configRepository = configRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
            _statRepository = statRepository;
        }

        public class GetResult
        {
            public string HomeTitle { get; set; }
            public bool IsSmsEnabled { get; set; }
            public bool IsUserCaptchaDisabled { get; set; }
            public bool IsUserRegistrationAllowed { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsSmsLogin { get; set; }
            public string Account { get; set; }
            public string Password { get; set; }
            public string Mobile { get; set; }
            public string Code { get; set; }
            public bool IsPersistent { get; set; }
            public string Token { get; set; }
            public string Value { get; set; }
        }

        public class SubmitResult
        {
            public bool RedirectToVerifyMobile { get; set; }
            public User User { get; set; }
            public string Token { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return _settingsManager.Cache.GetClassKey(typeof(LoginController), nameof(User), mobile);
        }
    }
}
