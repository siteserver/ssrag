using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Common.Form
{
    public partial class LayerFileUploadController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<Options>> Get([FromQuery] SiteRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var options = TranslateUtils.JsonDeserialize(site.Get<string>($"Home.{nameof(LayerFileUploadController)}"), new Options
            {
                IsChangeFileName = true
            });

            return options;
        }
    }
}
