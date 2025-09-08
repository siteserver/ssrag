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
    public partial class LostPasswordController : ControllerBase
    {
        public const string Route = "lostPassword";
        private const string RouteSendSms = "lostPassword/actions/sendSms";

        private readonly IAuthManager _authManager;
        private readonly ISettingsManager _settingsManager;
        private readonly ISmsManager _smsManager;
        private readonly IAdministratorRepository _administratorRepository;

        public LostPasswordController(IAuthManager authManager, ISettingsManager settingsManager, ISmsManager smsManager, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _settingsManager = settingsManager;
            _smsManager = smsManager;
            _administratorRepository = administratorRepository;
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
            return _settingsManager.Cache.GetClassKey(typeof(LostPasswordController), nameof(Administrator), mobile);
        }
    }
}