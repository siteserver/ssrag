using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("用户退出登录 API", "退出登录，使用POST发起请求，请求地址为/api/v1/users/actions/logout，此接口可以直接访问，无需通过身份验证。")]
        [HttpPost, Route(RouteActionsLogout)]
        public async Task<ActionResult<BoolResult>> Logout()
        {
            var cacheKey = Constants.GetSessionIdCacheKey(_authManager.UserName);
            await _dbCacheRepository.RemoveAsync(cacheKey);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
