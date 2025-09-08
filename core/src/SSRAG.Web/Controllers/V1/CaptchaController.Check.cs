using System;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Utils;
using System.Threading.Tasks;

namespace SSRAG.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        [OpenApiOperation("验证验证码 API", "验证验证码，使用POST发起请求，请求地址为/api/v1/captcha/actions/check，此接口可以直接访问，无需身份验证。")]
        [HttpPost, Route(RouteActionsCheck)]
        public async Task<ActionResult<BoolResult>> Check([FromBody] CheckRequest request)
        {
            var captcha = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(request.Value));

            if (captcha == null || string.IsNullOrEmpty(captcha.Value) || captcha.ExpireAt < DateTime.Now)
            {
                return this.Error("验证码已超时，请点击刷新验证码！");
            }

            if (!StringUtils.EqualsIgnoreCase(captcha.Value, request.Captcha) || await CaptchaUtils.IsAlreadyUsedAsync(captcha, _settingsManager))
            {
                return this.Error("验证码不正确，请重新输入！");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
