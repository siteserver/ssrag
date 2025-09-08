using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<ActionResult<TreeResult>> Tree([FromBody] TreeRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.SiteId);

            var enabledChannelIds = await _authManager.GetContentPermissionsChannelIdsAsync(site.Id);
            var visibleChannelIds = await _authManager.GetVisibleChannelIdsAsync(enabledChannelIds);

            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var visible = visibleChannelIds.Contains(summary.Id);
                if (!visible) return null;

                var disabled = !enabledChannelIds.Contains(summary.Id);
                var count = await _documentRepository.GetCountAsync(site.Id, summary.Id);

                return new
                {
                    Count = count,
                    Disabled = disabled
                };
            });

            if (!request.Reload)
            {
                var (cssUrls, jsUrls) = _pluginManager.GetExternalUrls();
                var siteUrl = _pathManager.GetSiteUrl(site, true);
                var groupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);
                var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);
                var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

                return new TreeResult
                {
                    Root = root,
                    CssUrls = cssUrls,
                    JsUrls = jsUrls,
                    SiteUrl = siteUrl,
                    GroupNames = groupNames,
                    TagNames = tagNames,
                    CheckedLevels = checkedLevels
                };
            }

            return new TreeResult
            {
                Root = root
            };
        }
    }
}
