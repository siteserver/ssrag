﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Home
{
    public partial class HomeMenusController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<UserMenusResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsHomeMenus))
            {
                return Unauthorized();
            }

            await _userMenuRepository.DeleteAsync(request.Id);

            return new UserMenusResult
            {
                UserMenus = await _userMenuRepository.GetUserMenusAsync()
            };
        }
    }
}
