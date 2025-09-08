using System.Collections.Generic;
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
    public partial class RegisterController : ControllerBase
    {
        private const string Route = "register";
        private const string RouteCaptcha = "register/captcha";
        private const string RouteCheckCaptcha = "register/captcha/actions/check";
        private const string RouteSendSms = "register/actions/sendSms";
        private const string RouteVerifyMobile = "register/actions/verifyMobile";

        private readonly ISettingsManager _settingsManager;
        private readonly ICloudManager _cloudManager;
        private readonly ISmsManager _smsManager;
        private readonly IConfigRepository _configRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IStatRepository _statRepository;

        public RegisterController(ISettingsManager settingsManager, ICloudManager cloudManager, ISmsManager smsManager, IConfigRepository configRepository, ITableStyleRepository tableStyleRepository, IUserRepository userRepository, IUserGroupRepository userGroupRepository, IStatRepository statRepository)
        {
            _settingsManager = settingsManager;
            _cloudManager = cloudManager;
            _smsManager = smsManager;
            _configRepository = configRepository;
            _tableStyleRepository = tableStyleRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _statRepository = statRepository;
        }

        public class GetResult
        {
            public bool IsSmsEnabled { get; set; }
            public bool IsUserVerifyMobile { get; set; }
            public bool IsUserRegistrationMobile { get; set; }
            public bool IsUserRegistrationEmail { get; set; }
            public bool IsUserRegistrationGroup { get; set; }
            public bool IsUserRegistrationDisplayName { get; set; }
            public bool IsUserCaptchaDisabled { get; set; }
            public bool IsHomeAgreement { get; set; }
            public string HomeAgreementHtml { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
            public IEnumerable<UserGroup> Groups { get; set; }
        }

        public class CheckRequest
        {
            public string Token { get; set; }
            public string Value { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        public class VerifyMobileRequest
        {
            public string Mobile { get; set; }
            public string Code { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return _settingsManager.Cache.GetClassKey(typeof(RegisterController), nameof(User), mobile);
        }
    }
}
