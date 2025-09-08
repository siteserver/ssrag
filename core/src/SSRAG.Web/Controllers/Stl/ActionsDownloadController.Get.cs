﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Stl
{
    public partial class ActionsDownloadController
    {
        [HttpGet, Route(Constants.RouteStlActionsDownload)]
        public async Task<ActionResult> Get([FromQuery] GetRequest request)
        {
            try
            {
                if (request.SiteId.HasValue && !string.IsNullOrEmpty(request.FileUrl) && !request.ContentId.HasValue)
                {
                    var fileUrl = _settingsManager.Decrypt(request.FileUrl);

                    if (PageUtils.IsProtocolUrl(fileUrl))
                    {
                        return Redirect(fileUrl);
                    }

                    var site = await _siteRepository.GetAsync(request.SiteId.Value);
                    var sitePath = _pathManager.GetSitePath(site);
                    var filePath = _pathManager.ParseSitePath(site, fileUrl);
                    if (!DirectoryUtils.IsInDirectory(sitePath, filePath))
                    {
                        return this.Error("下载失败，不存在此文件！");
                    }

                    if (_pathManager.IsFileDownload(site, PathUtils.GetExtension(filePath)))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            return this.Download(filePath);
                        }
                    }
                    else
                    {
                        var redirectUrl = _pathManager.ParseSiteUrl(site, fileUrl, false);
                        return Redirect(redirectUrl);
                    }
                }
                else if (request.SiteId.HasValue && !string.IsNullOrEmpty(request.FilePath))
                {
                    var site = await _siteRepository.GetAsync(request.SiteId.Value);
                    var sitePath = _pathManager.GetSitePath(site);
                    var filePath = _settingsManager.Decrypt(request.FilePath);
                    if (!DirectoryUtils.IsInDirectory(sitePath, filePath))
                    {
                        return this.Error("下载失败，不存在此文件！");
                    }

                    if (_pathManager.IsFileDownload(site, PathUtils.GetExtension(filePath)))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            return this.Download(filePath);
                        }
                    }
                    else
                    {
                        var fileUrl = _pathManager.GetRootUrlByPath(filePath);
                        return Redirect(_pathManager.ParseUrl(fileUrl));
                    }
                }
                else if (!string.IsNullOrEmpty(request.FilePath))
                {
                    var rootPath = _settingsManager.WebRootPath;
                    var filePath = _settingsManager.Decrypt(request.FilePath);
                    if (!DirectoryUtils.IsInDirectory(rootPath, filePath))
                    {
                        return this.Error("下载失败，不存在此文件！");
                    }

                    if (FileUtils.IsFileExists(filePath))
                    {
                        return this.Download(filePath);
                    }
                    else
                    {
                        var fileUrl = _pathManager.GetRootUrlByPath(filePath);
                        return Redirect(_pathManager.ParseUrl(fileUrl));
                    }
                }
                else if (request.SiteId.HasValue && request.ChannelId.HasValue && request.ContentId.HasValue && !string.IsNullOrEmpty(request.FileUrl))
                {
                    var fileUrl = _settingsManager.Decrypt(request.FileUrl);
                    var site = await _siteRepository.GetAsync(request.SiteId.Value);
                    var channel = await _channelRepository.GetAsync(request.ChannelId.Value);

                    await _contentRepository.AddDownloadsAsync(_channelRepository.GetTableName(site, channel), request.ChannelId.Value, request.ContentId.Value);

                    if (PageUtils.IsProtocolUrl(fileUrl))
                    {
                        return Redirect(fileUrl);
                    }

                    var sitePath = _pathManager.GetSitePath(site);
                    var filePath = _pathManager.ParseSitePath(site, fileUrl);
                    if (!DirectoryUtils.IsInDirectory(sitePath, filePath))
                    {
                        return this.Error("下载失败，不存在此文件！");
                    }

                    if (_pathManager.IsFileDownload(site, PathUtils.GetExtension(filePath)))
                    {
                        if (FileUtils.IsFileExists(filePath))
                        {
                            return this.Download(filePath);
                        }
                    }
                    else
                    {
                        var redirectUrl = _pathManager.ParseSiteUrl(site, fileUrl, false);
                        return Redirect(redirectUrl);
                    }
                }
            }
            catch
            {
                // ignored
            }

            return this.Error("下载失败，不存在此文件！");
        }
    }
}
