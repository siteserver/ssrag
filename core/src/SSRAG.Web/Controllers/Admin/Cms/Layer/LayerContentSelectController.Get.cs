using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Configuration;
using SSRAG.Utils;
using System.Collections.Generic;
using SSRAG.Models;
using System.Linq;

namespace SSRAG.Web.Controllers.Admin.Cms.Layer
{
    public partial class LayerContentSelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) || 
                !await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            var pageContents = new List<Content>();
            var contentIds = await _contentRepository.GetContentIdsCheckedAsync(site, channel);
            var total = contentIds.Count;
            
            if (total > 0)
            {
                var offset = site.PageSize * (request.Page - 1);
                var pageContentIds = contentIds.Skip(offset).Take(site.PageSize).ToList();

                foreach (var contentId in pageContentIds)
                {
                    if (contentId == request.ContentId) continue;
                    var content = await _contentRepository.GetAsync(site, request.ChannelId, contentId);
                    if (content == null) continue;

                    pageContents.Add(content);
                }
            }

            return new GetResult
            {
                PageContents = pageContents,
                Total = total,
                PageSize = site.PageSize,
            };
        }
    }
}