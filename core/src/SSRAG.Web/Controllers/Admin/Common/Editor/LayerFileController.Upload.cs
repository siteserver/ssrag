using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Editor
{
    public partial class LayerFileController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!_pathManager.IsFileExtensionAllowed(site, PathUtils.GetExtension(fileName)))
            {
                return this.Error(Constants.ErrorFileExtensionAllowed);
            }
            if (!_pathManager.IsFileSizeAllowed(site, file.Length))
            {
                return this.Error(Constants.ErrorFileSizeAllowed);
            }

            var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.File);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, fileName));

            await _pathManager.UploadAsync(file, filePath);

            var fileUrl = _pathManager.GetSiteUrlByPhysicalPath(site, filePath, true);
            var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Files);
            if (isAutoStorage)
            {
                var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                if (success)
                {
                    fileUrl = url;
                }
            }

            return new UploadResult
            {
                Name = fileName,
                Url = fileUrl
            };
        }
    }
}
