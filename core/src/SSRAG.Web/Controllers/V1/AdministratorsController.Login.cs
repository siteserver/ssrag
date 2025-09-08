using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("管理员登录 API", "注册新管理员，使用POST发起请求，请求地址为/api/v1/administrators/actions/login，此接口可以直接访问，无需通过身份验证。")]
        [HttpPost, Route(RouteActionsLogin)]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request)
        {
            var (administrator, userName, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);

            if (administrator == null)
            {
                administrator = await _administratorRepository.GetByUserNameAsync(userName);
                if (administrator != null)
                {
                    await _administratorRepository.UpdateLastActivityDateAndCountOfFailedLoginAsync(administrator); // 记录最后登录时间、失败次数+1
                }

                await _statRepository.AddCountAsync(StatType.AdminLoginFailure);
                return this.Error(errorMessage);
            }

            administrator = await _administratorRepository.GetByUserNameAsync(userName);
            await _administratorRepository.UpdateLastActivityDateAndCountOfLoginAsync(administrator); // 记录最后登录时间、失败次数清零

            var user = await _authManager.GetUserAsync();
            var token = await _authManager.AuthenticateAsync(administrator, user, request.IsAutoLogin);

            await _statRepository.AddCountAsync(StatType.AdminLoginSuccess);
            await _logRepository.AddAdminLogAsync(administrator, PageUtils.GetIpAddress(Request), Constants.ActionsLoginSuccess);

            var sessionId = StringUtils.Uuid();
            var cacheKey = Constants.GetSessionIdCacheKey(administrator.UserName);
            await _dbCacheRepository.RemoveAndInsertAsync(cacheKey, sessionId);

            var config = await _configRepository.GetAsync();

            var isEnforcePasswordChange = false;
            if (config.IsAdminEnforcePasswordChange)
            {
                if (administrator.LastChangePasswordDate == null)
                {
                    isEnforcePasswordChange = true;
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - administrator.LastChangePasswordDate.Value.Ticks);
                    if (ts.TotalDays > config.AdminEnforcePasswordChangeDays)
                    {
                        isEnforcePasswordChange = true;
                    }
                }
            }

            return new LoginResult
            {
                Administrator = administrator,
                AccessToken = token,
                SessionId = sessionId,
                IsEnforcePasswordChange = isEnforcePasswordChange
            };
        }
    }
}
