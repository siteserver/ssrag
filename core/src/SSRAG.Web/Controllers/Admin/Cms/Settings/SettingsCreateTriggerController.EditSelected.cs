using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateTriggerController
    {
        [HttpPost, Route(RouteEditSelected)]
        public async Task<ActionResult<BoolResult>> EditSelected([FromBody] EditSelectedRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsCreateTrigger))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            foreach (var channelId in request.ChannelIds)
            {
                var channel = await _channelRepository.GetAsync(channelId);
                channel.CreateChannelIdsIfContentChanged = ListUtils.ToString(request.CreateChannelIdsIfContentChanged);
                await _channelRepository.UpdateAsync(channel);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量设置栏目变动生成页面");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}