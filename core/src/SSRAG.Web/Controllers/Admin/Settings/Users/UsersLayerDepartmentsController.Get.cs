﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Configuration;
using SSRAG.Utils;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerDepartmentsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var userIds = ListUtils.GetIntList(request.UserIds);

            var users = new List<User>();
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByUserIdAsync(userId);
                if (user == null) continue;

                user.DepartmentName = await _departmentRepository.GetFullNameAsync(user.DepartmentId);
                users.Add(user);
            }

            return new GetResult
            {
                Users = users
            };
        }
    }
}