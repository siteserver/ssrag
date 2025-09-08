﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleAddController
    {
        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<BoolResult>> UpdateRole([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var role = await _roleRepository.GetRoleAsync(request.RoleId);
            if (role.RoleName != request.RoleName)
            {
                if (_roleRepository.IsPredefinedRole(request.RoleName))
                {
                    return this.Error($"角色添加失败，{request.RoleName}为系统角色！");
                }
                if (await _roleRepository.IsRoleExistsAsync(request.RoleName))
                {
                    return this.Error("角色名称已存在，请更换角色名称！");
                }
            }

            await _permissionsInRolesRepository.DeleteAsync(role.RoleName);
            await _sitePermissionsRepository.DeleteAsync(role.RoleName);

            if (request.AppPermissions != null && request.AppPermissions.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRoles
                {
                    Id = 0,
                    RoleName = request.RoleName,
                    AppPermissions = request.AppPermissions
                };
                await _permissionsInRolesRepository.InsertAsync(permissionsInRolesInfo);
            }

            if (request.SitePermissions != null && request.SitePermissions.Count > 0)
            {
                foreach (var sitePermissionsInfo in request.SitePermissions)
                {
                    sitePermissionsInfo.RoleName = request.RoleName;
                    await _sitePermissionsRepository.InsertAsync(sitePermissionsInfo);
                }
            }

            role.RoleName = request.RoleName;
            role.Description = request.Description;

            await _roleRepository.UpdateRoleAsync(role);

            await _settingsManager.Cache.ClearAsync();

            await _authManager.AddAdminLogAsync("修改管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}