using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home
{
    public partial class LostPasswordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var codeCacheKey = GetSmsCodeCacheKey(request.Mobile);
            var code = await _settingsManager.Cache.GetIntAsync(codeCacheKey);
            if (code == 0 || TranslateUtils.ToInt(request.Code) != code)
            {
                return this.Error("输入的验证码有误或验证码已超时");
            }

            var user = await _userRepository.GetByMobileAsync(request.Mobile);

            if (user == null)
            {
                return this.Error("此手机号码未关联用户");
            }

            if (!user.MobileVerified)
            {
                user.MobileVerified = true;
                await _userRepository.UpdateAsync(user);
            }

            var password = request.Password;
            var (isValid, errorMessage) = await _userRepository.ChangePasswordAsync(user.UserName, password);
            if (!isValid)
            {
                return this.Error($"更改密码失败：{errorMessage}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
