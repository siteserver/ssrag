﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class LoginController
    {
        [HttpPost, Route(RouteSendSms)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BoolResult>> SendSms([FromBody] SendSmsRequest request)
        {
            var administrator = await _administratorRepository.GetByMobileAsync(request.Mobile);

            if (administrator == null)
            {
                return this.Error("此手机号码未关联管理员，请更换手机号码");
            }

            var (success, errorMessage) = await _administratorRepository.ValidateLockAsync(administrator);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var code = StringUtils.GetRandomInt(100000, 999999);
            (success, errorMessage) =
                await _smsManager.SendSmsAsync(request.Mobile, SmsCodeType.LoginConfirmation, code);
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
