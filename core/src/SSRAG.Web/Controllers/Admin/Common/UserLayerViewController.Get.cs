using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Models;
using SSRAG.Configuration;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Common
{
    public partial class UserLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsUser) &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            User user = null;
            if (!string.IsNullOrEmpty(request.Uuid))
            {
                user = await _userRepository.GetByUuidAsync(request.Uuid);
            }

            if (user == null) return this.Error(Constants.ErrorNotFound);

            user.Remove("confirmPassword");

            // var groupName = await _userGroupRepository.GetUserGroupNameAsync(user.GroupId);

            // return new GetResult
            // {
            //     User = user,
            //     GroupName = groupName
            // };

            var groups = await _usersInGroupsRepository.GetGroupsAsync(user);
            var departmentFullName = await _departmentRepository.GetFullNameAsync(user.DepartmentId);

            return new GetResult
            {
                User = user,
                Groups = groups,
                DepartmentFullName = departmentFullName
            };
        }
    }
}
