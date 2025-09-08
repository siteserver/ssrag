﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home
{
    public partial class ProfileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] User request)
        {
            if (request.UserName != _authManager.UserName) return Unauthorized();

            var user = await _authManager.GetUserAsync();
            if (!PageUtils.IsProtocolUrl(request.AvatarUrl))
            {
                user.AvatarUrl = request.AvatarUrl;
            }
            user.Mobile = request.Mobile;
            user.Email = request.Email;

            var styles = await _tableStyleRepository.GetUserStylesAsync();

            foreach (var style in styles)
            {
                user.Set(style.AttributeName, request.Get(style.AttributeName));
            }

            var (success, errorMessage) = await _userRepository.UpdateAsync(user);
            if (!success)
            {
                return this.Error($"修改资料失败：{errorMessage}");
            }

            await _authManager.AddUserLogAsync("修改资料");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
