using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Editor
{
    public partial class LayerImageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<Options>> Get([FromQuery] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerImageController)), new Options
            {
                IsThumb = false,
                ThumbWidth = 1024,
                ThumbHeight = 1024,
                IsLinkToOriginal = true
            });

            return options;
        }
    }
}