﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsErrorController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsError))
            {
                return Unauthorized();
            }

            await _errorLogRepository.DeleteAllAsync();

            await _authManager.AddAdminLogAsync("清空错误日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
