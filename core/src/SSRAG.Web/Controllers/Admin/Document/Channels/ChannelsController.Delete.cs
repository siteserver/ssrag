using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<List<int>>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定父栏目");

            var channelIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId, request.ChannelId, ScopeType.All);

            if (request.DeleteFiles)
            {
                await _createManager.DeleteChannelsAsync(site, channelIdList);
            }

            var adminName = _authManager.AdminName;

            foreach (var channelId in channelIdList)
            {
                await _contentRepository.TrashContentsAsync(site, channelId, adminName);
            }

            foreach (var channelId in channelIdList)
            {
                await _channelRepository.DeleteAsync(site, channelId);
            }

            await _dbCacheRepository.ClearAllExceptAdminSessionsAsync();

            await _authManager.AddSiteLogAsync(request.SiteId, "删除栏目", $"栏目：{channel.ChannelName}");

            return new List<int>
            {
                request.SiteId,
                channel.ParentId
            };
        }
    }
}
