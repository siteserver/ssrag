using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Core.Utils.Serialization
{
    public static class BackupUtility
    {
        public const string UploadFolderName = "upload"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件夹目录
        public const string UploadFileName = "upload.xml"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件名

        public static async Task BackupTemplatesAsync(IPathManager pathManager, IDatabaseManager databaseManager, Caching caching, Site site, string filePath)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);
            await exportObject.ExportTemplatesAsync(filePath);
        }

        public static async Task BackupChannelsAndContentsAsync(IPathManager pathManager, IDatabaseManager databaseManager, Caching caching, Site site, string filePath)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);

            var channelIdList = await databaseManager.ChannelRepository.GetChannelIdsAsync(site.Id, site.Id, ScopeType.Children);

            await exportObject.ExportChannelsAsync(channelIdList, filePath, true, true, true);
        }

        public static void BackupFiles(IPathManager pathManager, IDatabaseManager databaseManager, Caching caching, Site site, string filePath)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);

            exportObject.ExportFiles(filePath);
        }

        public static async Task BackupSiteAsync(IPathManager pathManager, IDatabaseManager databaseManager, Caching caching, Site site, string filePath)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);

            var siteTemplateDir = PathUtils.GetFileNameWithoutExtension(filePath);
            var siteTemplatePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
            FileUtils.DeleteFileIfExists(filePath);
            var metadataPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);

            await exportObject.ExportFilesToSiteAsync(siteTemplatePath, true, null, null, true);

            var siteContentDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.Templates.SiteContent);
            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, true, true, new List<int>());

            var templateFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.Templates.FileTemplate);
            await exportObject.ExportTemplatesAsync(templateFilePath);
            var tableDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.Templates.Table);
            await exportObject.ExportTablesAndStylesAsync(tableDirectoryPath);
            var configurationFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.Templates.FileConfiguration);
            await exportObject.ExportConfigurationAsync(configurationFilePath);
            exportObject.ExportMetadata(site.SiteName, pathManager.GetWebUrl(site), string.Empty, string.Empty, metadataPath);

            pathManager.CreateZip(filePath, siteTemplatePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
        }

        public static async Task RecoverySiteAsync(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, Caching caching, Site site, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string adminName, string uuid)
        {
            var importObject = new ImportObject(pathManager, databaseManager, caching, site, adminName);

            var siteTemplatePath = path;
            if (isZip)
            {
                //解压文件
                siteTemplatePath = pathManager.GetTemporaryFilesPath(BackupType.Site.GetValue());
                DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

                pathManager.ExtractZip(path, siteTemplatePath);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.SiteTemplateMetadata);

            if (isDeleteChannels)
            {
                var channelIdList = await databaseManager.ChannelRepository.GetChannelIdsAsync(site.Id, site.Id, ScopeType.Children);
                foreach (var channelId in channelIdList)
                {
                    await databaseManager.ContentRepository.TrashContentsAsync(site, channelId, adminName);
                    await databaseManager.ChannelRepository.DeleteAsync(site, channelId);
                }
            }
            if (isDeleteTemplates)
            {
                var summaries = await databaseManager.TemplateRepository.GetSummariesAsync(site.Id);
                foreach (var summary in summaries)
                {
                    if (summary.DefaultTemplate == false)
                    {
                        await databaseManager.TemplateRepository.DeleteAsync(pathManager, site, summary.Id);
                    }
                }
            }
            if (isDeleteFiles)
            {
                await pathManager.DeleteSiteFilesAsync(site);
            }

            //导入文件
            await importObject.ImportFilesAsync(siteTemplatePath, isOverride, uuid);

            //导入模板
            var templateFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteFiles.Templates.FileTemplate);
            await importObject.ImportTemplatesAsync(templateFilePath, isOverride, adminName, uuid);

            //导入辅助表
            var tableDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteFiles.Templates.Table);

            //导入知识库设置
            var configurationFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteFiles.Templates.FileConfiguration);
            await importObject.ImportConfigurationAsync(configurationFilePath, uuid);

            //导入栏目及内容
            var siteContentDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteFiles.Templates.SiteContent);
            await importObject.ImportChannelsAndContentsAsync(0, siteContentDirectoryPath, isOverride, uuid);

            //导入表样式及清除缓存
            if (isUseTable)
            {
                await importObject.ImportTableStylesAsync(tableDirectoryPath, uuid);
            }

            await settingsManager.Cache.ClearAsync();
        }
    }
}
