using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home
{
    public partial class LostPasswordController
    {
        [HttpPost, Route(RouteSendSms)]
        public async Task<ActionResult<BoolResult>> SendSms([FromBody] SendSmsRequest request)
        {
            var user = await _userRepository.GetByMobileAsync(request.Mobile);

            if (user == null)
            {
                return this.Error("此手机号码未关联用户");
            }

            var (success, errorMessage) = await _userRepository.ValidateStateAsync(user);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var code = StringUtils.GetRandomInt(100000, 999999);
            (success, errorMessage) =
                await _smsManager.SendSmsAsync(request.Mobile, SmsCodeType.ChangePassword, code);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var cacheKey = GetSmsCodeCacheKey(request.Mobile);
            await _settingsManager.Cache.SetIntAsync(cacheKey, code, 10);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
