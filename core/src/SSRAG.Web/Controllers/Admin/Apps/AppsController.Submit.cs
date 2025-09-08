using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Dto;
using System;

namespace SSRAG.Web.Controllers.Admin.Apps
{
    public partial class AppsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<IntResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafeMode);
            }

            var siteId = request.SiteId;

            if (request.SiteId > 0)
            {
                var site = await _siteRepository.GetAsync(request.SiteId);

                site.SiteName = request.SiteName;
                site.Description = request.Description;
                site.IconUrl = request.IconUrl;

                if (site.Root == false)
                {
                    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetDirectoryName(site.SiteDir, false), request.SiteDir))
                    {
                        var list = await _siteRepository.GetSiteDirsAsync();
                        if (ListUtils.ContainsIgnoreCase(list, request.SiteDir))
                        {
                            return this.Error("站点修改失败，已存在相同的发布路径！");
                        }

                        var parentPsPath = _settingsManager.WebRootPath;
                        DirectoryUtility.ChangeSiteDir(parentPsPath, site.SiteDir, request.SiteDir);
                    }

                    site.SiteDir = request.SiteDir;
                }

                await _siteRepository.UpdateAsync(site);

                await _authManager.AddAdminLogAsync("修改站点属性", $"站点:{site.SiteName}");
            }
            else
            {
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

                string errorMessage;
                (siteId, errorMessage) = await _siteRepository.InsertSiteAsync(channel, new Site
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

                if (await _authManager.IsSiteAdminAsync() && !await _authManager.IsSuperAdminAsync())
                {
                    var siteIdList = await _authManager.GetSiteIdsAsync() ?? new List<int>();
                    siteIdList.Add(siteId);
                    var adminInfo = await _administratorRepository.GetByUserNameAsync(adminName);
                    await _administratorRepository.UpdateSiteIdsAsync(adminInfo, siteIdList);
                }
            }

            return new IntResult
            {
                Value = siteId
            };
        }
    }
}