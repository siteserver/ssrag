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
        [HttpGet, Route(RouteActionsListFile)]
        public async Task<ActionResult<ListFileResult>> ListFile([FromQuery] ListFileRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);

            var directoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.File);

            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Where(x =>
                _pathManager.IsFileExtensionAllowed(site, PathUtils.GetExtension(x))).OrderByDescending(x => x);

            var list = new List<FileResult>();
            foreach (var x in files.Skip(request.Start).Take(request.Size))
            {
                list.Add(new FileResult
                {
                    Url = _pathManager.GetSiteUrlByPhysicalPath(site, x, true)
                });
            }

            return new ListFileResult
            {
                State = "SUCCESS",
                Size = request.Size,
                Start = request.Start,
                Total = files.Count(),
                List = list
            };
        }

        public class ListFileRequest : SiteRequest
        {
            public int Start { get; set; }
            public int Size { get; set; }
        }

        public class FileResult
        {
            public string Url { get; set; }
        }

        public class ListFileResult
        {
            public string State { get; set; }
            public int Start { get; set; }
            public int Size { get; set; }
            public int Total { get; set; }
            public IEnumerable<FileResult> List { get; set; }
        }
    }
}
