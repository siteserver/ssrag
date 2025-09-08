﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsLayerReferenceController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var content = await _contentRepository.GetAsync(site, request.ChannelId, request.ContentId);
            var sourceSiteId = await _channelRepository.GetSiteIdAsync(content.SourceId);
            var sourceName = await SourceManager.GetSourceNameAsync(_databaseManager, content);

            return new GetResult
            {
                Content = content,
                SourceSiteId = sourceSiteId,
                SourceName = sourceName
            };
        }
    }
}