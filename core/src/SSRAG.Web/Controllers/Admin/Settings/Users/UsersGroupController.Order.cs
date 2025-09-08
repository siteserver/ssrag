using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        [HttpPost, Route(RouteOrder)]
        public async Task<ActionResult<GetResult>> Order([FromBody] OrderRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            for (int i = 0; i < request.Rows; i++)
            {
                var group = await _userGroupRepository.GetAsync(request.GroupId);
                if (group == null) continue;
                
                if (request.IsUp)
                {
                    await _userGroupRepository.UpdateTaxisUpAsync(group.Id, group.Taxis);
                }
                else
                {
                    await _userGroupRepository.UpdateTaxisDownAsync(group.Id, group.Taxis);
                }
            }

            var groups = await _userGroupRepository.GetUserGroupsAsync(true);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}