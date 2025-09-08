﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class DashboardController
    {
        [HttpGet, Route(RouteUnCheckedList)]
        public async Task<ActionResult<GetUnCheckedListResult>> GetUnCheckedList()
        {
            var unCheckedList = new List<UnChecked>();

            if (await _authManager.IsSuperAdminAsync())
            {
                foreach (var site in await _siteRepository.GetSitesAsync())
                {
                    var count = await _contentRepository.GetCountCheckingAsync(site);
                    if (count > 0)
                    {
                        unCheckedList.Add(new UnChecked
                        {
                            SiteId = site.Id,
                            SiteName = site.SiteName,
                            Count = count
                        });
                    }
                }
            }
            else
            {
                var siteIdListWithPermissions = await _authManager.GetSiteIdsAsync();
                if (siteIdListWithPermissions != null)
                {
                    foreach (var siteId in siteIdListWithPermissions)
                    {
                        var isCheckable = await _authManager.HasSitePermissionsAsync(siteId, MenuUtils.SitePermissions.ContentsCheck);
                        if (!isCheckable) continue;

                        var site = await _siteRepository.GetAsync(siteId);
                        if (site == null) continue;

                        var count = 0;
                        if (await _authManager.IsSiteAdminAsync(siteId))
                        {
                            count = await _contentRepository.GetCountCheckingAsync(site);
                        }
                        else
                        {
                            var channelIds = await _authManager.GetContentPermissionsChannelIdsAsync(siteId, MenuUtils.ContentPermissions.CheckLevel1, MenuUtils.ContentPermissions.CheckLevel2, MenuUtils.ContentPermissions.CheckLevel3, MenuUtils.ContentPermissions.CheckLevel4, MenuUtils.ContentPermissions.CheckLevel5);
                            count = await _contentRepository.GetCountCheckingAsync(site, channelIds);
                        }

                        if (count > 0)
                        {
                            unCheckedList.Add(new UnChecked
                            {
                                SiteId = site.Id,
                                SiteName = site.SiteName,
                                Count = count
                            });
                        }
                    }
                }
            }

            var totalCount = unCheckedList.Sum(x => x.Count);

            return new GetUnCheckedListResult
            {
                UnCheckedList = unCheckedList,
                TotalCount = totalCount
            };
        }
    }
}