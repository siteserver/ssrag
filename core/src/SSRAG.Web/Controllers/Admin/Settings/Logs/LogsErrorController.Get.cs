﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsErrorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SearchResult>> Get([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsError))
            {
                return Unauthorized();
            }

            return await GetResultsAsync(request);
        }
    }
}
