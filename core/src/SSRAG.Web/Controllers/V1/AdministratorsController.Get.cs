using System.Threading.Tasks;
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
        [OpenApiOperation("获取管理员 API", "获取管理员，使用GET发起请求，请求地址为/api/v1/administrators/{id}")]
        [HttpGet, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Get([FromRoute] int id)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators))
            {
                return Unauthorized();
            }
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            if (!await _administratorRepository.IsExistsAsync(id)) return this.Error(Constants.ErrorNotFound);

            var administrator = await _administratorRepository.GetByUserIdAsync(id);

            return administrator;
        }
    }
}
