using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsLayerPasswordController : ControllerBase
    {
        private const string Route = "settings/administratorsLayerPassword";

        private readonly IAuthManager _authManager;
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorsLayerPasswordController(IAuthManager authManager, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _administratorRepository = administratorRepository;
        }

        public class GetRequest
        {
            public string UserName { get; set; }
        }

        public class GetResult
        {
            public Administrator Administrator { get; set; }
            public bool OldPassword { get; set; }
        }

        public class SubmitRequest
        {
            public string UserName { get; set; }
            public string OldPassword { get; set; }
            public string Password { get; set; }
        }
    }
}
