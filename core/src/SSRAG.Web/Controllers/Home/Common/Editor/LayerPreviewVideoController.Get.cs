using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Common.Editor
{
    public partial class LayerPreviewVideoController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var rootUrl = _pathManager.GetRootUrl();
            var siteUrl = _pathManager.GetSiteUrl(site, true);

            return new GetResult
            {
                RootUrl = rootUrl,
                SiteUrl = siteUrl
            };
        }
    }
}