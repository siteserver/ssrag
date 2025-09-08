using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateRuleController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var node = await _channelRepository.GetAsync(summary.Id);
                var filePath = await _pathManager.GetInputChannelUrlAsync(site, node, false);
                var contentFilePathRule = string.IsNullOrEmpty(node.ContentFilePathRule)
                    ? await _pathManager.GetContentFilePathRuleAsync(site, summary.Id)
                    : node.ContentFilePathRule;

                return new
                {
                    Channel = node,
                    Count = count,
                    FilePath = filePath,
                    ContentFilePathRule = contentFilePathRule
                };
            });

            return new GetResult
            {
                Channel = cascade
            };
        }
    }
}