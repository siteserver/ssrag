﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("用户登录 API", "用户登录，使用POST发起请求，请求地址为/api/v1/users/actions/login，此接口可以直接访问，无需身份验证")]
        [HttpPost, Route(RouteActionsLogin)]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request)
        {
            User user = null;
            var errorMessage = Constants.ErrorNotFound;

            if (!string.IsNullOrEmpty(request.OpenId))
            {
                user = await _userRepository.GetByOpenIdAsync(request.OpenId);
            }
            else if (!string.IsNullOrEmpty(request.Account) && !string.IsNullOrEmpty(request.Password))
            {
                (user, _, errorMessage) = await _userRepository.ValidateAsync(request.Account, request.Password, true);
            }

            if (user == null)
            {
                return this.Error(errorMessage);
            }

            var admin = await _authManager.GetAdminAsync();
            var accessToken = await _authManager.AuthenticateAsync(admin, user, request.IsPersistent);

            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(user);

            await _statRepository.AddCountAsync(StatType.UserLogin);
            await _logRepository.AddUserLogAsync(user, PageUtils.GetIpAddress(Request), Constants.ActionsLoginSuccess);

            return new LoginResult
            {
                User = user,
                AccessToken = accessToken
            };
        }
    }
}
