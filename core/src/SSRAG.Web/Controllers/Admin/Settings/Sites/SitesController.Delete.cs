﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Core.Utils;
using SSRAG.Configuration;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<SitesResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafeMode);
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (!StringUtils.EqualsIgnoreCase(site.SiteDir, StringUtils.Trim(request.SiteDir)))
            {
                return this.Error("删除失败，请输入正确的文件夹名称");
            }

            if (request.DeleteFiles)
            {
                await _pathManager.DeleteSiteFilesAsync(site);
            }
            await _authManager.AddAdminLogAsync("删除站点", $"站点:{site.SiteName}");

            var list = await _channelRepository.GetChannelIdsAsync(request.SiteId);
            await _tableStyleRepository.DeleteAllAsync(site.TableName, list);
            await _channelGroupRepository.DeleteAllAsync(request.SiteId);
            await _contentGroupRepository.DeleteAllAsync(request.SiteId);
            await _contentTagRepository.DeleteAllAsync(request.SiteId);
            await _contentCheckRepository.DeleteAllAsync(request.SiteId);
            await _formRepository.DeleteAllAsync(request.SiteId);
            await _formDataRepository.DeleteAllAsync(request.SiteId);
            await _relatedFieldRepository.DeleteAllAsync(request.SiteId);
            await _relatedFieldItemRepository.DeleteAllAsync(request.SiteId);
            await _sitePermissionsRepository.DeleteAllAsync(request.SiteId);
            await _specialRepository.DeleteAllAsync(request.SiteId);
            await _statRepository.DeleteAllAsync(request.SiteId);
            await _templateLogRepository.DeleteAllAsync(request.SiteId);
            await _templateRepository.DeleteAllAsync(request.SiteId);
            await _translateRepository.DeleteAllAsync(request.SiteId);

            await _channelRepository.DeleteAllAsync(request.SiteId);
            await _siteRepository.DeleteAsync(request.SiteId);

            var siteIdList = await _siteRepository.GetSiteIdsAsync();
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                var info = await _siteRepository.GetAsync(id);
                if (info != null)
                {
                    sites.Add(info);
                }
            }

            return new SitesResult
            {
                Sites = sites
            };
        }
    }
}