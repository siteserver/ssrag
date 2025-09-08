﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Services
{
    partial interface IPathManager
    {
        //根据发布系统属性判断是否为相对路径并返回解析后路径
        string ParseSiteUrl(Site site, string url, bool isLocal);

        string ParseSitePath(Site site, string virtualPath);

        string GetSiteUrl(Site site, bool isLocal);

        string GetSiteUrl(Site site, string requestPath, bool isLocal);

        string GetPreviewSiteUrl(int siteId);

        string GetPreviewChannelUrl(int siteId, int channelId);

        string GetPreviewContentUrl(int siteId, int channelId, int contentId, bool isPreview = false);

        string GetPreviewFileUrl(int siteId, int fileTemplateId);

        string GetPreviewSpecialUrl(int siteId, int specialId);

        string GetSiteUrlByPhysicalPath(Site site, string physicalPath, bool isLocal);

        string GetVirtualUrlByPhysicalPath(Site site, string physicalPath);

        string GetRemoteSiteUrl(Site site, string requestPath);

        string GetLocalSiteUrl(Site site, string requestPath = null);

        // 得到发布系统首页地址
        Task<string> GetIndexPageUrlAsync(Site site, bool isLocal);

        Task<string> GetFileUrlAsync(Site site, int fileTemplateId, bool isLocal);

        Task<string> GetContentUrlAsync(Site site, Content content, bool isLocal);

        Task<string> GetContentUrlAsync(Site site, Channel channel, int contentId, bool isLocal);

        Task<string> GetContentUrlByIdAsync(Site site, Content contentCurrent, bool isLocal);

        Task<string> GetContentUrlByIdAsync(Site site, int channelId, int contentId, int sourceId, int referenceId, LinkType linkType, string linkUrl, bool isLocal);

        Task<string> GetChannelUrlNotComputedAsync(Site site, int channelId, bool isLocal);

        //得到栏目经过计算后的连接地址
        Task<string> GetChannelUrlAsync(Site site, Channel channel, bool isLocal);

        Task<string> GetBaseUrlAsync(Site site, Template template, int channelId, int contentId);

        string RemoveDefaultFileName(Site site, string url);

        Task<string> GetInputChannelUrlAsync(Site site, Channel node, bool isLocal);

        string AddVirtualToUrl(string url);

        string GetVirtualUrl(Site site, string url);

        bool IsVirtualUrl(string url);

        bool IsRelativeUrl(string url);

        List<Select<string>> GetLinkTypeSelects(bool isChannel);

        string GetSitePath(Site site);

        string GetSitePath(Site site, params string[] paths);

        Task<string> GetSitePathAsync(int siteId, params string[] paths);

        string GetIndexPageFilePath(Site site, string createFileFullName, bool root);

        string GetBackupFilePath(Site site, BackupType backupType);

        string GetUploadDirectoryPath(Site site, string fileExtension);

        string GetUploadDirectoryPath(Site site, DateTime datetime, string fileExtension);

        List<FileInfo> GetAllFilesOrderByCreationTimeDesc(Site site, UploadType uploadType);

        string GetUploadDirectoryPath(Site site, UploadType uploadType);

        string GetUploadDirectoryPath(Site site, DateTime datetime, UploadType uploadType);

        string GetUploadFileName(Site site, string filePath);

        Task<Site> GetSiteAsync(string path);

        Task<string> GetSiteDirAsync(string path);

        Task<int> GetCurrentSiteIdAsync();

        string AddVirtualToPath(string path);

        //将编辑器中图片上传至本机
        Task<string> SaveImageAsync(Site site, string content, string excludePrefix = null);

        string GetTemporaryFilesPath(string relatedPath);

        string GetSiteTemplatesPath(SiteType siteType, string relatedPath);

        string GetSiteTemplateMetadataPath(string siteTemplatePath, string relatedPath);

        Task<string> GetChannelFilePathRuleAsync(Site site, int channelId);

        Task<string> GetChannelFilePathRuleAsync(int siteId, int channelId);

        Task<string> GetContentFilePathRuleAsync(Site site, int channelId);

        Task<string> GetContentFilePathRuleAsync(int siteId, int channelId);

        string GetPageFilePathAsync(string filePath, int currentPageIndex);

        Task<string> GetChannelPageFilePathAsync(Site site, int channelId);

        Task<string> GetContentPageFilePathAsync(Site site, int channelId, int contentId, int currentPageIndex);

        Task<string> GetContentPageFilePathAsync(Site site, int channelId, Content content, int currentPageIndex);

        bool IsImageExtensionAllowed(Site site, string fileExtension);

        bool IsImageSizeAllowed(Site site, long contentLength);

        bool IsVideoExtensionAllowed(Site site, string fileExtension);

        bool IsVideoSizeAllowed(Site site, long contentLength);

        bool IsAudioExtensionAllowed(Site site, string fileExtension);

        bool IsAudioSizeAllowed(Site site, long contentLength);

        bool IsFileExtensionAllowed(Site site, string fileExtension);

        bool IsFileSizeAllowed(Site site, long contentLength);

        bool IsFileDownload(Site site, string fileExtension);

        string GetBinDirectoryPath(string relatedPath);

        string PhysicalSiteFilesPath { get; }

        Task DeleteSiteFilesAsync(Site site);

        Task<(bool success, string errorMessage)> ChangeParentSiteAsync(int oldParentSiteId, int newParentSiteId, int siteId, string siteDir);

        Task ChangeToRootAsync(Site site, bool isMoveFiles);

        Task ChangeToSubSiteAsync(Site site, string siteDir, IList<string> directories, IList<string> files);

        bool IsSystemDirectory(string directoryName);

        void AddWaterMark(Site site, string imagePath);

        void MoveFile(Site sourceSite, Site destSite, string relatedUrl);

        void MoveFileByChannel(Site sourceSite, Site destSite, Channel channel);

        Task MoveFileByContentAsync(Site sourceSite, Site destSite, Content content);
    }
}
