using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Core.Utils;
using SSRAG.Configuration;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsLayerArrangeController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Arrange))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            await _contentRepository.UpdateArrangeTaxisAsync(site, channel, request.AttributeName, request.IsDesc);

            await _authManager.AddSiteLogAsync(request.SiteId, "批量整理", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}