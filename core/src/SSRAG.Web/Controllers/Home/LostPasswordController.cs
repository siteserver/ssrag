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
    public partial class LostPasswordController : ControllerBase
    {
        public const string Route = "lostPassword";
        private const string RouteSendSms = "lostPassword/actions/sendSms";


        private readonly IAuthManager _authManager;
        private readonly ISmsManager _smsManager;
        private readonly IUserRepository _userRepository;
        private readonly ISettingsManager _settingsManager;

        public LostPasswordController(IAuthManager authManager, ISmsManager smsManager, IUserRepository userRepository, ISettingsManager settingsManager)
        {
            _authManager = authManager;
            _smsManager = smsManager;
            _userRepository = userRepository;
            _settingsManager = settingsManager;
        }

        public class SubmitRequest
        {
            public string Mobile { get; set; }
            public string Code { get; set; }
            public string Password { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return _settingsManager.Cache.GetClassKey(typeof(LostPasswordController), nameof(User), mobile);
        }
    }
}