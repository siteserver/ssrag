using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Common.Editor
{
    public partial class ActionsController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteActionsUploadVideo)]
        public async Task<ActionResult<UploadVideoResult>> UploadVideo([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();
            if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return new UploadVideoResult
                {
                    Error = Constants.ErrorUpload
                };
            }

            var original = Path.GetFileName(file.FileName);
            var fileName = _pathManager.GetUploadFileName(site, original);

            if (!_pathManager.IsVideoExtensionAllowed(site, PathUtils.GetExtension(fileName)))
            {
                return new UploadVideoResult
                {
                    Error = Constants.ErrorVideoExtensionAllowed
                };
            }
            if (!_pathManager.IsVideoSizeAllowed(site, file.Length))
            {
                return new UploadVideoResult
                {
                    Error = Constants.ErrorVideoSizeAllowed
                };
            }

            var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.Video);
            var filePath = PathUtils.Combine(localDirectoryPath, fileName);

            await _pathManager.UploadAsync(file, filePath);

            var fileUrl = _pathManager.GetSiteUrlByPhysicalPath(site, filePath, true);

            return new UploadVideoResult
            {
                State = "SUCCESS",
                Url = fileUrl,
                Title = original,
                Original = original,
                Error = null
            };
        }

        public class UploadVideoResult
        {
            public string State { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string Original { get; set; }
            public string Error { get; set; }
        }
    }
}
