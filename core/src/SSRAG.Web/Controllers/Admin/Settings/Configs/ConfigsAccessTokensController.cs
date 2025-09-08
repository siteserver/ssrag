using System.Collections.Generic;
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
    public partial class ConfigsAccessTokensController : ControllerBase
    {
        private const string Route = "settings/configsAccessTokens";
        private const string RouteDelete = "settings/configsAccessTokens/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public ConfigsAccessTokensController(IAuthManager authManager, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
        }

        public class ListResult
        {
            public List<AccessToken> Tokens { get; set; }
            public List<string> AdminNames { get; set; }
            public List<string> Scopes { get; set; }
            public string AdminName { get; set; }
        }

        public class TokensResult
        {
            public List<AccessToken> Tokens { get; set; }
        }
    }
}
