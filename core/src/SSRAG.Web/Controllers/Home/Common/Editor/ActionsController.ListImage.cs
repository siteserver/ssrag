using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Common.Editor
{
    public partial class ActionsController
    {
        [HttpGet, Route(RouteActionsListImage)]
        public async Task<ActionResult<ListImageResult>> ListImage([FromQuery] ListImageRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);

            var directoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.Image);

            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Where(x =>
                _pathManager.IsImageExtensionAllowed(site, PathUtils.GetExtension(x))).OrderByDescending(x => x);

            var list = new List<ImageResult>();
            foreach (var x in files.Skip(request.Start).Take(request.Size))
            {
                list.Add(new ImageResult
                {
                    Url = _pathManager.GetSiteUrlByPhysicalPath(site, x, true)
                });
            }

            return new ListImageResult
            {
                State = "SUCCESS",
                Size = request.Size,
                Start = request.Start,
                Total = files.Count(),
                List = list
            };
        }

        public class ListImageRequest : SiteRequest
        {
            public int Start { get; set; }
            public int Size { get; set; }
        }

        public class ImageResult
        {
            public string Url { get; set; }
        }

        public class ListImageResult
        {
            public string State { get; set; }
            public int Start { get; set; }
            public int Size { get; set; }
            public int Total { get; set; }
            public IEnumerable<ImageResult> List { get; set; }
        }
    }
}
