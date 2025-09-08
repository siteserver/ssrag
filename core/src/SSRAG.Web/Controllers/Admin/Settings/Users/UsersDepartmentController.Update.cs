using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersDepartmentController
    {
        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<List<int>>> Update([FromBody] Department request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersDepartment))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                return this.Error("部门修改失败，必须填写部门名称！");
            }

            var department = await _departmentRepository.GetAsync(request.Id);

            department.Name = request.Name;
            department.Description = request.Description;

            await _departmentRepository.UpdateAsync(department);

            var expendedDepartmentIds = new List<int>();
            if (!expendedDepartmentIds.Contains(department.ParentId))
            {
                expendedDepartmentIds.Add(department.ParentId);
            }

            return expendedDepartmentIds;
        }
    }
}