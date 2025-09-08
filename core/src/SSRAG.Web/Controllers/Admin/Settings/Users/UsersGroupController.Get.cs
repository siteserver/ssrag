using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Models;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            var allGroups = await _userGroupRepository.GetUserGroupsAsync(true);
            var groups = new List<UserGroup>();
            foreach (var group in allGroups)
            {
                var admin = await _administratorRepository.GetByUserNameAsync(group.AdminName);
                if (admin != null)
                {
                    group.Set("AdminName", _administratorRepository.GetDisplay(admin));
                    group.Set("AdminUuid", admin.Uuid);
                }
                groups.Add(group);
            }

            return new GetResult
            {
                Groups = groups,
                AdminNames = await _administratorRepository.GetUserNamesAsync()
            };
        }
    }
}