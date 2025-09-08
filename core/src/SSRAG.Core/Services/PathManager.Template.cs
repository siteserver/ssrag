using System.IO;
using System.Threading.Tasks;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class PathManager
    {
        public string GetTemplateFilePath(Site site, Template template)
        {
            string filePath;
            if (template.TemplateType == TemplateType.IndexPageTemplate)
            {
                filePath = GetSitePath(site, template.RelatedFileName);
            }
            else if (template.TemplateType == TemplateType.ContentTemplate)
            {
                filePath = GetSitePath(site, DirectoryUtils.Site.Template, DirectoryUtils.Site.Content, template.RelatedFileName);
            }
            else
            {
                filePath = GetSitePath(site, DirectoryUtils.Site.Template, template.RelatedFileName);
            }
            return filePath;
        }

        public async Task<string> GetTemplateContentAsync(Site site, Template template)
        {
            var filePath = GetTemplateFilePath(site, template);
            return await GetContentByFilePathAsync(filePath);
        }

        public async Task WriteContentToTemplateFileAsync(Site site, Template template, string content, string adminName)
        {
            if (content == null) content = string.Empty;
            var filePath = GetTemplateFilePath(site, template);
            await FileUtils.WriteTextAsync(filePath, content);

            if (template.Id > 0)
            {
                var logInfo = new TemplateLog
                {
                    Id = 0,
                    TemplateId = template.Id,
                    SiteId = template.SiteId,
                    AdminName = adminName,
                    ContentLength = content.Length,
                    TemplateContent = content
                };
                await _templateLogRepository.InsertAsync(logInfo);
            }
        }

        public async Task<string> GetIncludeContentAsync(Site site, string file)
        {
            var filePath = ParseSitePath(site, AddVirtualToPath(file));

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists) return string.Empty;
            filePath = fileInfo.FullName;

            if (IsInRootDirectory(filePath))
            {
                return await GetContentByFilePathAsync(filePath);
            }
            return string.Empty;
        }

        public async Task WriteContentToIncludeFileAsync(Site site, string file, string content)
        {
            if (content == null) content = string.Empty;
            var filePath = GetSitePath(site, file);
            await FileUtils.WriteTextAsync(filePath, content);
        }

        public async Task<string> GetContentByFilePathAsync(string filePath)
        {
            return await _settingsManager.Cache.GetByFilePathAsync(filePath);
        }
    }
}
