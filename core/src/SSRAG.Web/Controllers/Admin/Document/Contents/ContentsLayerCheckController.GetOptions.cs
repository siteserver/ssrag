﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsLayerCheckController
    {
        [HttpPost, Route(RouteOptions)]
        public async Task<ActionResult<GetOptionsResult>> GetOptions([FromBody] GetOptionsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ContentsCheck))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channelIdList = await _authManager.GetContentPermissionsChannelIdsAsync(request.TransSiteId, MenuUtils.ContentPermissions.Add);

            var transChannels = await _channelRepository.GetAsync(request.TransSiteId);
            var transSite = await _siteRepository.GetAsync(request.TransSiteId);
            var cascade = await _channelRepository.GetCascadeAsync(transSite, transChannels, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);

                return new
                {
                    Disabled = !channelIdList.Contains(summary.Id),
                    summary.IndexName,
                    Count = count
                };
            });

            return new GetOptionsResult
            {
                TransChannels = cascade
            };
        }
    }
}