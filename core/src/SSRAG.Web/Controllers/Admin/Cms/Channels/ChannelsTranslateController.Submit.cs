using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsTranslateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ChannelsTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var adminName = _authManager.AdminName;
            foreach (var transChannelId in request.TransChannelIds)
            {
                await TranslateAsync(site, request.TransSiteId, transChannelId, request.TranslateType, request.ChannelIds, request.IsDeleteAfterTranslate, adminName);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
