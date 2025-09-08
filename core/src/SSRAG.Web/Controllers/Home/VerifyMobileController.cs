using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class VerifyMobileController : ControllerBase
    {
        private const string Route = "verifyMobile";
        private const string RouteSendSms = "verifyMobile/actions/sendSms";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ISmsManager _smsManager;
        private readonly IUserRepository _userRepository;

        public VerifyMobileController(ISettingsManager settingsManager, IAuthManager authManager, ISmsManager smsManager, IUserRepository userRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _smsManager = smsManager;
            _userRepository = userRepository;
        }

        public class GetResult
        {
            public string Mobile { get; set; }
        }

        public class SubmitRequest
        {
            public string Mobile { get; set; }
            public string Code { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return _settingsManager.Cache.GetClassKey(typeof(VerifyMobileController), nameof(User), mobile);
        }
    }
}
