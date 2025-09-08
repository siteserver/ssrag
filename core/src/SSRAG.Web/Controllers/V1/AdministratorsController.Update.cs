﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("修改管理员 API", "修改管理员属性，使用POST发起请求，请求地址为/api/v1/administrators/{id}")]
        [HttpPost, Route(RouteAdministratorUpdate)]
        public async Task<ActionResult<Administrator>> Update([FromRoute] int id, [FromBody] Administrator administrator)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators))
            {
                return Unauthorized();
            }
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            if (administrator == null) return this.Error("Could not read administrator from body");

            if (!await _administratorRepository.IsExistsAsync(id)) return this.Error(Constants.ErrorNotFound);

            administrator.Id = id;

            var (isValid, errorMessage) = await _administratorRepository.UpdateAsync(administrator);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return administrator;
        }
    }
}
