﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsCheckController
    {
        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<ListResult>> List([FromBody] ListRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.ContentsCheck))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.SiteId);

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.CheckContents);

            var channelIds = request.ChannelIds;
            if (!await _authManager.IsSuperAdminAsync() && !await _authManager.IsSiteAdminAsync(request.SiteId))
            {
                if (request.ChannelIds.Count == 1 && request.ChannelIds[0] == request.SiteId)
                {
                    channelIds = await _authManager.GetContentPermissionsChannelIdsAsync(site.Id);
                }
            }

            var pageContents = new List<Content>();
            var (total, pageSummaries) = await _contentRepository.CheckSearchAsync(site, request.Page, channelIds, request.IsAllContents, request.StartDate, request.EndDate, request.Items, true, request.CheckedLevels, request.IsTop, request.IsRecommend, request.IsHot, request.IsColor, request.GroupNames, request.TagNames);

            if (total > 0)
            {
                var offset = site.PageSize * (request.Page - 1);

                var sequence = offset + 1;
                foreach (var summary in pageSummaries)
                {
                    var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                    if (content == null) continue;

                    var pageContent =
                        await columnsManager.CalculateContentListAsync(sequence++, site, request.SiteId, content, columns);

                    pageContents.Add(pageContent);
                }
            }

            return new ListResult
            {
                PageContents = pageContents,
                Total = total,
                PageSize = site.PageSize
            };
        }
    }
}
