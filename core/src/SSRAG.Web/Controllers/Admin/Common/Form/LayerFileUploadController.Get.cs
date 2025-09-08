using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Form
{
    public partial class LayerFileUploadController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<Options>> Get([FromQuery] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var isChangeFileName = site.IsFileUploadChangeFileName;

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerFileUploadController)), new Options
            {
                IsChangeFileName = isChangeFileName,
            });

            return options;
        }
    }
}
