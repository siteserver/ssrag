using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class PasswordController : ControllerBase
    {
        private const string Route = "password";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;

        public PasswordController(IAuthManager authManager, IUserRepository userRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
        }

        public class GetResult
        {
            public User User { get; set; }
        }

        public class SubmitRequest
        {
            public string Password { get; set; }
        }
    }
}
