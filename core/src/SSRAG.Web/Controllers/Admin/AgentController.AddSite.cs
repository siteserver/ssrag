using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Enums;
using System;
using System.Collections.Generic;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteAddSite)]
        public async Task<ActionResult<AddSiteResult>> AddSite([FromBody] AddSiteRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
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

                var rootPath = _pathManager.GetRootPath();
                var directories = DirectoryUtils.GetDirectoryNames(rootPath);
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

            var tableName = await _contentRepository.CreateNewContentTableAsync();
            var (siteId, errorMessage) = await _siteRepository.InsertSiteAsync(channel, new Site
            {
                SiteName = request.SiteName,
                SiteType = SiteType.Web,
                SiteDir = request.SiteDir,
                TableName = tableName,
                Root = request.Root
            }, string.Empty);

            if (siteId == 0)
            {
                return this.Error(errorMessage);
            }

            var caching = new Caching(_settingsManager);
            var site = await _siteRepository.GetAsync(siteId);

            await caching.SetProcessAsync(request.Uuid, "任务初始化...");

            await caching.SetProcessAsync(request.Uuid, "开始下载模板压缩包，可能需要几分钟，请耐心等待...");

            var fileName = PageUtils.GetFileNameFromUrl(request.ThemeDownloadUrl);
            var filePath = _pathManager.GetSiteTemplatesPath(request.SiteType, fileName);
            await HttpClientUtils.DownloadAsync(request.ThemeDownloadUrl, filePath);

            await caching.SetProcessAsync(request.Uuid, "模板压缩包下载成功，开始解压缩，可能需要几分钟，请耐心等待...");

            var siteTemplateDir = PathUtils.GetFileNameWithoutExtension(filePath);
            var directoryPath = _pathManager.GetSiteTemplatesPath(request.SiteType, siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            _pathManager.ExtractZip(filePath, directoryPath);

            await caching.SetProcessAsync(request.Uuid, "模板压缩包解压成功，正在导入数据...");

            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching, request.SiteType);
            await manager.ImportSiteTemplateToEmptySiteAsync(site, siteTemplateDir, true, true, string.Empty, request.Uuid);

            await caching.SetProcessAsync(request.Uuid, "生成站点页面...");
            await _createManager.CreateByAllAsync(site.Id);

            await caching.SetProcessAsync(request.Uuid, "清除系统缓存...");
            await _settingsManager.Cache.ClearAsync();

            var retVal = site.Clone<Site>();
            retVal.Set("LocalUrl", _pathManager.GetSiteUrl(site, true));
            retVal.Set("SiteUrl", _pathManager.GetSiteUrl(site, false));

            return new AddSiteResult
            {
                Site = retVal
            };
        }
    }
}
