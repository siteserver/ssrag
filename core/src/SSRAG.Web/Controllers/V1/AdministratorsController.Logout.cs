using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Enums;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("管理员退出登录 API", "退出登录，使用POST发起请求，请求地址为/api/v1/administrators/actions/logout，此接口可以直接访问，无需通过身份验证。")]
        [HttpPost, Route(RouteActionsLogout)]
        public async Task<ActionResult<BoolResult>> Logout()
        {
            var cacheKey = Constants.GetSessionIdCacheKey(_authManager.AdminName);
            await _dbCacheRepository.RemoveAsync(cacheKey);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
