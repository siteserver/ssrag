using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Form
{
    public partial class LayerVideoUploadController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var vodSettings = await _vodManager.GetVodSettingsAsync();
            var isCloudVod = _vodManager is ICloudManager && vodSettings.IsVod;

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerVideoUploadController)), new Options
            {
                IsChangeFileName = true,
            });

            return new GetResult
            {
                IsChangeFileName = options.IsChangeFileName,
                IsCloudVod = isCloudVod,
                VideoUploadExtensions = site.VideoUploadExtensions
            };
        }
    }
}
