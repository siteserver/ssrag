﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home.Common.Form
{
    public partial class LayerImageUploadController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<SubmitResult>>> Submit([FromBody] SubmitRequest request)
        {
            var result = new List<SubmitResult>();
            if (request.SiteId > 0)
            {
                var siteIds = await _authManager.GetSiteIdsAsync();
                if (!ListUtils.Contains(siteIds, request.SiteId)) return Unauthorized();

                var site = await _siteRepository.GetAsync(request.SiteId);
                if (site == null) return this.Error("无法确定内容对应的知识库");

                var isAutoStorage = await _storageManager.IsAutoStorageAsync(request.SiteId, SyncType.Images);


                foreach (var filePath in request.FilePaths)
                {
                    if (string.IsNullOrEmpty(filePath)) continue;

                    var fileName = PathUtils.GetFileName(filePath);

                    var fileExtName = StringUtils.ToLower(PathUtils.GetExtension(filePath));
                    var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, fileExtName);

                    var virtualUrl = _pathManager.GetVirtualUrlByPhysicalPath(site, filePath);
                    var imageUrl = _pathManager.ParseSiteUrl(site, virtualUrl, true);
                    if (isAutoStorage)
                    {
                        var (success, url) = await _storageManager.StorageAsync(request.SiteId, filePath);
                        if (success)
                        {
                            virtualUrl = imageUrl = url;
                        }
                    }

                    if (request.IsThumb)
                    {
                        var localSmallFileName = Constants.SmallImageAppendix + fileName;
                        var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                        var thumbnailVirtualUrl = _pathManager.GetVirtualUrlByPhysicalPath(site, localSmallFilePath);
                        var thumbnailUrl = _pathManager.ParseSiteUrl(site, thumbnailVirtualUrl, true);
                        _pathManager.ResizeImageByMax(filePath, localSmallFilePath, request.ThumbWidth, request.ThumbHeight);

                        if (isAutoStorage)
                        {
                            var (success, url) = await _storageManager.StorageAsync(request.SiteId, localSmallFilePath);
                            if (success)
                            {
                                thumbnailVirtualUrl = thumbnailUrl = url;
                            }
                        }

                        if (request.IsLinkToOriginal)
                        {
                            result.Add(new SubmitResult
                            {
                                ImageUrl = thumbnailUrl,
                                ImageVirtualUrl = thumbnailVirtualUrl,
                                PreviewUrl = imageUrl,
                                PreviewVirtualUrl = virtualUrl
                            });
                        }
                        else
                        {
                            FileUtils.DeleteFileIfExists(filePath);
                            result.Add(new SubmitResult
                            {
                                ImageUrl = thumbnailUrl,
                                ImageVirtualUrl = thumbnailVirtualUrl
                            });
                        }
                    }
                    else
                    {
                        result.Add(new SubmitResult
                        {
                            ImageUrl = imageUrl,
                            ImageVirtualUrl = virtualUrl
                        });
                    }
                }

                var options = TranslateUtils.JsonDeserialize(site.Get<string>($"Home.{nameof(LayerImageUploadController)}"), new Options
                {
                    IsEditor = true,
                    IsThumb = false,
                    ThumbWidth = 1024,
                    ThumbHeight = 1024,
                    IsLinkToOriginal = true,
                });

                options.IsEditor = request.IsEditor;
                options.IsThumb = request.IsThumb;
                options.ThumbWidth = request.ThumbWidth;
                options.ThumbHeight = request.ThumbHeight;
                options.IsLinkToOriginal = request.IsLinkToOriginal;
                site.Set($"Home.{nameof(LayerImageUploadController)}", TranslateUtils.JsonSerialize(options));

                await _siteRepository.UpdateAsync(site);
            }
            else
            {
                foreach (var filePath in request.FilePaths)
                {
                    if (string.IsNullOrEmpty(filePath)) continue;

                    var fileName = PathUtils.GetFileName(filePath);
                    var userFilePath = _pathManager.GetUserUploadPath(_authManager.UserName, fileName);
                    FileUtils.CopyFile(filePath, userFilePath, true);

                    var imageUrl = _pathManager.GetUserUploadUrl(_authManager.UserName, fileName);
                    result.Add(new SubmitResult
                    {
                        ImageUrl = imageUrl,
                        ImageVirtualUrl = imageUrl
                    });
                }
            }

            return result;
        }
    }
}