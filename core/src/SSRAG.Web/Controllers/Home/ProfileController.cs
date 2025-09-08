using System.Collections.Generic;
using SSRAG.Datory;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ProfileController : ControllerBase
    {
        private const string Route = "profile";
        private const string RouteUpload = "profile/actions/upload";
        private const string RouteSendSms = "profile/actions/sendSms";
        private const string RouteVerifyMobile = "profile/actions/verifyMobile";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICloudManager _cloudManager;
        private readonly ISmsManager _smsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly ISettingsManager _settingsManager;

        public ProfileController(IAuthManager authManager, IPathManager pathManager, ICloudManager cloudManager, ISmsManager smsManager, IConfigRepository configRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository, IRelatedFieldItemRepository relatedFieldItemRepository, ISettingsManager settingsManager)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _cloudManager = cloudManager;
            _smsManager = smsManager;
            _configRepository = configRepository;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
            _settingsManager = settingsManager;
        }

        public class GetResult
        {
            public bool IsSmsEnabled { get; set; }
            public bool IsUserVerifyMobile { get; set; }
            public Entity Entity { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
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
            return _settingsManager.Cache.GetClassKey(typeof(ProfileController), nameof(User), mobile);
        }
    }
}
