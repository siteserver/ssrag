using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home
{
    public partial class ProfileController
    {
        [HttpPost, Route(RouteVerifyMobile)]
        public async Task<ActionResult<BoolResult>> VerifyMobile([FromBody] VerifyMobileRequest request)
        {
            var codeCacheKey = GetSmsCodeCacheKey(request.Mobile);
            var code = await _settingsManager.Cache.GetIntAsync(codeCacheKey);
            if (code == 0 || TranslateUtils.ToInt(request.Code) != code)
            {
                return this.Error("输入的验证码有误或验证码已超时");
            }

            var user = await _authManager.GetUserAsync();
            user.Mobile = request.Mobile;
            user.MobileVerified = true;

            var (success, errorMessage) = await _userRepository.UpdateAsync(user);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
