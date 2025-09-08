﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Configuration;
using SSRAG.Utils;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerDepartmentsController
    {
        [HttpPost, Route(RouteOptions)]
        public async Task<ActionResult<GetOptionsResult>> GetOptions([FromBody] GetOptionsRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var cascade = await _departmentRepository.GetCascadesAsync(0, async department =>
            {
                var count = await _departmentRepository.GetAllCountAsync(department);

                return new
                {
                    Count = count,
                };
            });

            return new GetOptionsResult
            {
                TransDepartments = cascade
            };
        }
    }
}