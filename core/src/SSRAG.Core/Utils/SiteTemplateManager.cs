using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Core.Utils.Serialization;
using SSRAG.Core.Utils.Serialization.Components;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Core.Utils
{
    public class SiteTemplateManager
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly Caching _caching;
        private readonly string _rootPath;

        public SiteTemplateManager(IPathManager pathManager, IDatabaseManager databaseManager, Caching caching, SiteType siteType)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _caching = caching;
            _rootPath = _pathManager.GetSiteTemplatesPath(siteType, string.Empty);
            DirectoryUtils.CreateDirectoryIfNotExists(_rootPath);
        }

        public void DeleteSiteTemplate(string siteTemplateDir)
        {
            var directoryPath = PathUtils.Combine(_rootPath, siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            var filePath = PathUtils.Combine(_rootPath, siteTemplateDir + ".zip");
            FileUtils.DeleteFileIfExists(filePath);
        }

        public void DeleteZipSiteTemplate(string fileName)
        {
            var filePath = PathUtils.Combine(_rootPath, fileName);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public bool IsSiteTemplateDirectoryExists(string siteTemplateDir)
        {
            var siteTemplatePath = PathUtils.Combine(_rootPath, siteTemplateDir);
            return DirectoryUtils.IsDirectoryExists(siteTemplatePath);
        }

        public List<SiteTemplate> GetSiteTemplates()
        {
            var siteTemplates = new List<SiteTemplate>();
            var directoryPaths = DirectoryUtils.GetDirectoryPaths(_rootPath);
            foreach (var siteTemplatePath in directoryPaths)
            {
                var directoryName = PathUtils.GetDirectoryName(siteTemplatePath, false);
                SiteTemplate siteTemplate = null;
                var metadataXmlFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.FileMetadata);
                if (FileUtils.IsFileExists(metadataXmlFilePath))
                {
                    siteTemplate = XmlUtils.ConvertFileToObject<SiteTemplate>(metadataXmlFilePath);
                }

                if (siteTemplate == null)
                {
                    siteTemplate = new SiteTemplate
                    {
                        SiteTemplateName = directoryName
                    };
                }

                siteTemplate.DirectoryName = directoryName;
                siteTemplates.Add(siteTemplate);
            }

            return siteTemplates.OrderBy(x => x.DirectoryName).ToList();
        }

        public List<string> GetZipSiteTemplateList()
        {
            var list = new List<string>();
            foreach (var fileName in DirectoryUtils.GetFileNames(_rootPath))
            {
                if (FileUtils.IsZip(PathUtils.GetExtension(fileName)))
                {
                    list.Add(fileName);
                }
            }
            return list;
        }

        public async Task ImportSiteTemplateToEmptySiteAsync(Site site, string siteTemplateDir, bool isImportContents, bool isImportTableStyles, string adminName, string uuid)
        {
            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(site.SiteType, siteTemplateDir);
            if (!DirectoryUtils.IsDirectoryExists(siteTemplatePath)) return;

            var templateFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.FileTemplate);
            var tableDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.Table);
            var configurationFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.FileConfiguration);
            var siteContentDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.SiteContent);
            var formDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.Form);

            var importObject = new ImportObject(_pathManager, _databaseManager, _caching, site, adminName);

            await _caching.SetProcessAsync(uuid, $"导入知识库文件: {siteTemplatePath}");
            await importObject.ImportFilesAsync(siteTemplatePath, true, uuid);

            await _caching.SetProcessAsync(uuid, $"导入模板文件: {templateFilePath}");
            await importObject.ImportTemplatesAsync(templateFilePath, true, adminName, uuid);

            var filePathList = ImportObject.GetSiteContentFilePathList(siteContentDirectoryPath);
            foreach (var filePath in filePathList)
            {
                await _caching.SetProcessAsync(uuid, $"导入栏目文件: {filePath}");
                await importObject.ImportSiteContentAsync(siteContentDirectoryPath, filePath, isImportContents, uuid);
            }

            var channels = await _databaseManager.ChannelRepository.GetChannelsAsync(site.Id);
            foreach (var channel in channels)
            {
                var contentIds = await _databaseManager.ContentRepository.GetContentIdsByLinkTypeAsync(site, channel, LinkType.LinkToChannel);
                foreach (var contentId in contentIds)
                {
                    var content = await _databaseManager.ContentRepository.GetAsync(site, channel, contentId);
                    var linkToChannelName = content.Get<string>(ContentIe.LinkToChannelName);
                    if (!string.IsNullOrEmpty(linkToChannelName))
                    {
                        var linkToChannel = channels.FirstOrDefault(x => x.ChannelName == linkToChannelName);
                        if (linkToChannel != null)
                        {
                            content.LinkUrl = ListUtils.ToString(linkToChannel.ParentsPath) + "," + linkToChannel.Id;
                            await _databaseManager.ContentRepository.UpdateAsync(site, channel, content);
                        }
                    }
                }
            }

            if (isImportTableStyles)
            {
                await _caching.SetProcessAsync(uuid, $"导入表字段: {tableDirectoryPath}");
                await importObject.ImportTableStylesAsync(tableDirectoryPath, uuid);
            }

            await _caching.SetProcessAsync(uuid, $"导入表单: {formDirectoryPath}");
            await importObject.ImportFormsAsync(formDirectoryPath, uuid);

            await _caching.SetProcessAsync(uuid, $"导入配置文件: {configurationFilePath}");
            await importObject.ImportConfigurationAsync(configurationFilePath, uuid);
        }

        public static async Task ExportSiteToSiteTemplateAsync(IPathManager pathManager, IDatabaseManager databaseManager, Caching caching, Site site, string siteTemplateDir)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);

            var siteTemplatePath = pathManager.GetSiteTemplatesPath(site.SiteType, siteTemplateDir);

            //导出模板
            var templateFilePath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.FileTemplate);
            await exportObject.ExportTemplatesAsync(templateFilePath);
            //导出辅助表及样式
            var tableDirectoryPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.Table);
            await exportObject.ExportTablesAndStylesAsync(tableDirectoryPath);
            //导出知识库属性以及知识库属性表单
            var configurationFilePath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.FileConfiguration);
            await exportObject.ExportConfigurationAsync(configurationFilePath);
            //导出关联字段
            var relatedFieldDirectoryPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.RelatedField);
            await exportObject.ExportRelatedFieldAsync(relatedFieldDirectoryPath);
            //导出表单
            var formDirectoryPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.Form);
            await exportObject.ExportFormsAsync(formDirectoryPath);
        }
    }
}
