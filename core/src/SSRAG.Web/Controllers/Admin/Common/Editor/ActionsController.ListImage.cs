using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Editor
{
    public partial class ActionsController
    {
        [HttpGet, Route(RouteActionsListImage)]
        public async Task<ActionResult<ListImageResult>> ListImage([FromQuery] ListImageRequest request)
        {
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
    }
}
