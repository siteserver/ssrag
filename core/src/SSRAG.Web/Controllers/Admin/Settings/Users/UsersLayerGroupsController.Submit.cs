using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerGroupsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var userIds = ListUtils.GetIntList(request.UserIds);
            foreach (var userId in userIds)
            {
                foreach (var groupId in request.GroupIds)
                {
                    if (request.IsCancel)
                    {
                        await _usersInGroupsRepository.DeleteAsync(groupId, userId);
                    }
                    else
                    {
                        await _usersInGroupsRepository.InsertIfNotExistsAsync(groupId, userId);
                    }
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}