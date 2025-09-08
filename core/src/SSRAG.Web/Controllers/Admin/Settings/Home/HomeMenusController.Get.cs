using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Home
{
    public partial class HomeMenusController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsHomeMenus))
            {
                return Unauthorized();
            }

            var userMenus = await _userMenuRepository.GetUserMenusAsync();
            var groups = await _userGroupRepository.GetUserGroupsAsync(true);
            
            return new GetResult
            {
                UserMenus = userMenus,
                Groups = groups
            };
        }
    }
}
