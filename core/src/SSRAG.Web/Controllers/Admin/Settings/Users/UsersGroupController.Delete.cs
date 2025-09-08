using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            var groupName = await _userGroupRepository.GetNameAsync(request.Id);
            await _userGroupRepository.DeleteAsync(request.Id);
            await _usersInGroupsRepository.DeleteAllByGroupIdAsync(request.Id);
            await _authManager.AddAdminLogAsync("删除用户组", $"用户组:{groupName}");

            return new GetResult
            {
                Groups = await _userGroupRepository.GetUserGroupsAsync(true)
            };
        }
    }
}