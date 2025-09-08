using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("文件不存在！");
            }

            var fileUrl = string.Empty;

            if (request.Type == nameof(Models.Content.ImageUrl))
            {
                var (success, filePath, errorMessage) = await _pathManager.UploadImageAsync(site, file);
                if (!success)
                {
                    return this.Error(errorMessage);
                }

                fileUrl = _pathManager.GetSiteUrlByPhysicalPath(site, filePath, true);
                var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Images);
                if (isAutoStorage)
                {
                    (success, var url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                    if (success)
                    {
                        fileUrl = url;
                    }
                }
            }
            else if (request.Type == nameof(Models.Content.VideoUrl))
            {
                var fileName = Path.GetFileName(file.FileName);
                if (!_pathManager.IsVideoExtensionAllowed(site, PathUtils.GetExtension(fileName)))
                {
                    return this.Error(Constants.ErrorVideoExtensionAllowed);
                }
                if (!_pathManager.IsVideoSizeAllowed(site, file.Length))
                {
                    return this.Error(Constants.ErrorVideoSizeAllowed);
                }

                var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.Video);
                var localFileName = PathUtils.GetUploadFileName(fileName, true);
                var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

                await _pathManager.UploadAsync(file, filePath);
                fileUrl = _pathManager.GetSiteUrlByPhysicalPath(site, filePath, true);
            }
            else if (request.Type == nameof(Models.Content.FileUrl))
            {
                var fileName = PathUtils.GetFileName(file.FileName);
                if (!_pathManager.IsFileExtensionAllowed(site, PathUtils.GetExtension(fileName)))
                {
                    return this.Error(Constants.ErrorFileExtensionAllowed);
                }
                if (!_pathManager.IsFileSizeAllowed(site, file.Length))
                {
                    return this.Error(Constants.ErrorFileSizeAllowed);
                }

                var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.File);
                var localFileName = PathUtils.GetUploadFileName(fileName, true);
                var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

                await _pathManager.UploadAsync(file, filePath);
                fileUrl = _pathManager.GetSiteUrlByPhysicalPath(site, filePath, true);
            }

            return new StringResult
            {
                Value = fileUrl
            };
        }
    }
}
