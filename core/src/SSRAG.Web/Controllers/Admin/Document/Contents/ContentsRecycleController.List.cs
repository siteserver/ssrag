﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsRecycleController
    {
        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<ListResult>> List([FromBody] ListRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.ContentsRecycle))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.RecycleContents);

            var pageContents = new List<Content>();
            var (total, pageSummaries) = await _contentRepository.RecycleSearchAsync(site, request.Page, request.ChannelId, request.StartDate, request.EndDate, request.Items, request.IsCheckedLevels, request.CheckedLevels, request.IsTop, request.IsRecommend, request.IsHot, request.IsColor, request.GroupNames, request.TagNames);

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
