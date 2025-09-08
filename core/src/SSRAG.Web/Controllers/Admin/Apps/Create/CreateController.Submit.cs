using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Dto;
using System;

namespace SSRAG.Web.Controllers.Admin.Apps.Create
{
    public partial class CreateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<IntResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            if (!request.Root)
            {
                if (_pathManager.IsSystemDirectory(request.SiteDir))
                {
                    return this.Error("文件夹名称不能为系统文件夹名称，请更改文件夹名称！");
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return this.Error("文件夹名称不符合系统要求，请更改文件夹名称！");
                }
                if (string.IsNullOrEmpty(request.SiteDir) || request.SiteDir == "/")
                {
                    var isRootExists = await _siteRepository.IsRootExistsAsync();
                    if (isRootExists)
                    {
                        return this.Error("已存在根站点，请更改文件夹名称！");
                    }
                }

                var sitePath = _pathManager.GetRootPath();
                var directories = DirectoryUtils.GetDirectoryNames(sitePath);
                if (ListUtils.ContainsIgnoreCase(directories, request.SiteDir))
                {
                    return this.Error("已存在相同的文件夹，请更改文件夹名称！");
                }
                var list = await _siteRepository.GetSiteDirsAsync();
                if (ListUtils.ContainsIgnoreCase(list, request.SiteDir))
                {
                    return this.Error("已存在相同的站点文件夹，请更改文件夹名称！");
                }
            }

            var channel = new Channel();

            channel.ChannelName = channel.IndexName = "首页";
            channel.ParentId = 0;
            channel.ContentModelPluginId = string.Empty;
            channel.ParentsPath = new List<int>();
            channel.AddDate = DateTime.Now;

            var adminName = _authManager.AdminName;

            var (siteId, errorMessage) = await _siteRepository.InsertSiteAsync(channel, new Site
            {
                SiteName = request.SiteName,
                SiteType = request.SiteType,
                SiteDir = request.SiteDir,
                TableName = string.Empty,
                Root = request.Root
            }, adminName);

            if (siteId == 0)
            {
                return this.Error(errorMessage);
            }

            var tableName = await _contentRepository.CreateNewContentTableAsync();
            await _siteRepository.UpdateTableNameAsync(siteId, tableName);

            if (await _authManager.IsSiteAdminAsync() && !await _authManager.IsSuperAdminAsync())
            {
                var siteIdList = await _authManager.GetSiteIdsAsync() ?? new List<int>();
                siteIdList.Add(siteId);
                var adminInfo = await _administratorRepository.GetByUserNameAsync(adminName);
                await _administratorRepository.UpdateSiteIdsAsync(adminInfo, siteIdList);
            }

            var caching = new Caching(_settingsManager);
            var site = await _siteRepository.GetAsync(siteId);

            await caching.SetProcessAsync(request.Uuid, "任务初始化...");

            if (!string.IsNullOrEmpty(request.SiteTemplate))
            {
                var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching, request.SiteType);
                await manager.ImportSiteTemplateToEmptySiteAsync(site, request.SiteTemplate, true, true, adminName, request.Uuid);

                await caching.SetProcessAsync(request.Uuid, "生成站点页面...");
                await _createManager.CreateByAllAsync(site.Id);

                await caching.SetProcessAsync(request.Uuid, "清除系统缓存...");
                await _settingsManager.Cache.ClearAsync();
            }
            else
            {
                var templates = await _templateRepository.GetSummariesAsync(site.Id);
                foreach (var summary in templates)
                {
                    var template = await _templateRepository.GetAsync(summary.Id);
                    await _pathManager.WriteContentToTemplateFileAsync(site, template, Constants.Html5Empty, adminName);
                }

                await _createManager.CreateByAllAsync(site.Id);
            }

            return new IntResult
            {
                Value = siteId
            };
        }
    }
}