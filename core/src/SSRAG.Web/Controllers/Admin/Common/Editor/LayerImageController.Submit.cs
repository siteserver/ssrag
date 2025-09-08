using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Editor
{
    public partial class LayerImageController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<SubmitResult>>> Submit([FromBody] SubmitRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的知识库");

            var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Images);

            var result = new List<SubmitResult>();
            foreach (var path in request.FilePaths)
            {
                if (string.IsNullOrEmpty(path)) continue;

                var filePath = _pathManager.ParsePath(path);
                var fileName = PathUtils.GetFileName(filePath);

                var fileExtName = StringUtils.ToLower(PathUtils.GetExtension(filePath));
                var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, fileExtName);

                var imageUrl = _pathManager.GetSiteUrlByPhysicalPath(site, filePath, true);
                if (isAutoStorage)
                {
                    var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                    if (success)
                    {
                        imageUrl = url;
                    }
                }

                if (request.IsThumb)
                {
                    var localSmallFileName = Constants.SmallImageAppendix + fileName;
                    var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                    var width = request.ThumbWidth;
                    var height = request.ThumbHeight;
                    ImageUtils.MakeThumbnail(filePath, localSmallFilePath, width, height, true);

                    var thumbnailUrl = _pathManager.GetSiteUrlByPhysicalPath(site, localSmallFilePath, true);
                    if (isAutoStorage)
                    {
                        var (success, url) = await _storageManager.StorageAsync(request.SiteId, localSmallFilePath);
                        if (success)
                        {
                            thumbnailUrl = url;
                        }
                    }

                    if (request.IsLinkToOriginal)
                    {
                        result.Add(new SubmitResult
                        {
                            ImageUrl = thumbnailUrl,
                            PreviewUrl = imageUrl
                        });
                    }
                    else
                    {
                        FileUtils.DeleteFileIfExists(filePath);
                        result.Add(new SubmitResult
                        {
                            ImageUrl = thumbnailUrl
                        });
                    }
                }
                else
                {
                    result.Add(new SubmitResult
                    {
                        ImageUrl = imageUrl
                    });
                }
            }

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerImageController)), new Options
            {
                IsThumb = false,
                ThumbWidth = 1024,
                ThumbHeight = 1024,
                IsLinkToOriginal = true,
            });

            options.IsThumb = request.IsThumb;
            options.ThumbWidth = request.ThumbWidth;
            options.ThumbHeight = request.ThumbHeight;
            options.IsLinkToOriginal = request.IsLinkToOriginal;
            site.Set(nameof(LayerImageController), TranslateUtils.JsonSerialize(options));

            await _siteRepository.UpdateAsync(site);

            return result;
        }
    }
}