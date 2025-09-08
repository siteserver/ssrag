using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Utils;
using System.Collections.Generic;
using System;

namespace SSRAG.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsSiteController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            var fileName = $"知识库日志_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.xlsx";
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);
            var head = new List<string>
            {
                "知识库名称",
                "管理员",
                "IP地址",
                "日期",
                "动作",
                "描述"
            };
            var rows = new List<List<string>>();

            var results = await GetResultsAsync(request);
            foreach (var item in results.Items)
            {
                rows.Add(new List<string>
                {
                    item.SiteName,
                    item.AdminName,
                    item.IpAddress,
                    DateUtils.GetDateAndTimeString(item.CreatedDate),
                    item.Action,
                    item.Summary
                });
            }

            ExcelUtils.Write(filePath, head, rows);
            var downloadUrl = _pathManager.GetTemporaryFilesUrl(fileName);
            await _authManager.AddAdminLogAsync("导出知识库日志");

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
