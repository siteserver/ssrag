using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersGroupController : ControllerBase
    {
        private const string Route = "settings/usersGroup";
        private const string RouteDelete = "settings/usersGroup/actions/delete";
        private const string RouteOrder = "settings/usersGroup/actions/order";

        private readonly IAuthManager _authManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUsersInGroupsRepository _usersInGroupsRepository;

        public UsersGroupController(
            IAuthManager authManager,
            ISettingsManager settingsManager,
            IAdministratorRepository administratorRepository,
            IUserGroupRepository userGroupRepository,
            IUsersInGroupsRepository usersInGroupsRepository
        )
        {
            _authManager = authManager;
            _settingsManager = settingsManager;
            _administratorRepository = administratorRepository;
            _userGroupRepository = userGroupRepository;
            _usersInGroupsRepository = usersInGroupsRepository;
        }

        public class GetResult
        {
            public IEnumerable<UserGroup> Groups { get; set; }
            public IEnumerable<string> AdminNames { get; set; }
        }

        public class OrderRequest
        {
            public int GroupId { get; set; }
            public bool IsUp { get; set; }
            public int Rows { get; set; }
        }
    }
}
