using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Utils;
using System.Linq;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Home.Common.Form
{
    public partial class LayerImageSelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var files = _pathManager.GetAllFilesOrderByCreationTimeDesc(site, UploadType.Image);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                files = files.Where(file => StringUtils.ContainsIgnoreCase(file.Name, request.Keyword)).ToList();
            }
            var count = files.Count;
            var images = new List<Image>();

            var skip = (request.Page - 1) * request.PerPage;
            files = files.Skip(skip).Take(request.PerPage).ToList();
            foreach (var file in files)
            {
                var virtualUrl = _pathManager.GetVirtualUrlByPhysicalPath(site, file.FullName);
                var imageUrl = _pathManager.ParseSiteUrl(site, virtualUrl, true);

                images.Add(new Image
                {
                    ImageUrl = imageUrl,
                    VirtualUrl = virtualUrl
                });
            }

            return new GetResult
            {
                Count = count,
                Images = images,
            };
        }
    }
}