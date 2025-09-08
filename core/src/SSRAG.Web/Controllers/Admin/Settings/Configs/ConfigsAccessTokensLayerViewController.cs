using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Configs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ConfigsAccessTokensLayerViewController : ControllerBase
    {
        private const string Route = "settings/configsAccessTokensLayerView";
        private const string RouteRegenerate = "settings/configsAccessTokensLayerView/actions/regenerate";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IAccessTokenRepository _accessTokenRepository;

        public ConfigsAccessTokensLayerViewController(ISettingsManager settingsManager, IAuthManager authManager, IAccessTokenRepository accessTokenRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _accessTokenRepository = accessTokenRepository;
        }

        public class GetResult
        {
            public AccessToken Token { get; set; }
            public string AccessToken { get; set; }
        }

        public class RegenerateResult
        {
            public string AccessToken { get; set; }
        }
    }
}
