using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class FormManager
    {
        public const string MAIN_FILE_NAME = "index.html";

        private string GetTemplatesDirectoryPath()
        {
            return _pathManager.GetSiteFilesPath("assets/forms");
        }

        private string GetTemplatesDirectoryPath(Site site)
        {
            return _pathManager.GetSitePath(site, "forms");
        }

        public string GetTemplateDirectoryPath(Site site, bool isSystem, string name)
        {
            if (isSystem)
            {
                var directoryPath = GetTemplatesDirectoryPath();
                return PathUtils.Combine(directoryPath, name);
            }
            else
            {
                var directoryPath = GetTemplatesDirectoryPath(site);
                return PathUtils.Combine(directoryPath, name);
            }
        }

        public async Task<string> GetTemplateHtmlAsync(Site site, bool isSystem, string name)
        {
            if (isSystem)
            {
                var directoryPath = GetTemplatesDirectoryPath();
                var htmlPath = PathUtils.Combine(directoryPath, name, MAIN_FILE_NAME);
                return await _pathManager.GetContentByFilePathAsync(htmlPath);
            }
            else
            {
                var directoryPath = GetTemplatesDirectoryPath(site);
                var htmlPath = PathUtils.Combine(directoryPath, name, MAIN_FILE_NAME);
                return await _pathManager.GetContentByFilePathAsync(htmlPath);
            }
        }

        public void SetTemplateHtml(Site site, string name, string html)
        {
            var directoryPath = GetTemplatesDirectoryPath(site);
            var htmlPath = PathUtils.Combine(directoryPath, name, MAIN_FILE_NAME);
            FileUtils.WriteText(htmlPath, html);
        }

        public void DeleteTemplate(Site site, string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            var directoryPath = GetTemplatesDirectoryPath(site);
            var templatePath = PathUtils.Combine(directoryPath, name);
            DirectoryUtils.DeleteDirectoryIfExists(templatePath);
        }

        public List<FormTemplate> GetFormTemplates(Site site)
        {
            var templates = new List<FormTemplate>();

            var directoryPath = GetTemplatesDirectoryPath(site);
            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                var directoryNames = DirectoryUtils.GetDirectoryNames(directoryPath);
                foreach (var directoryName in directoryNames)
                {
                    var template = GetFormTemplate(directoryPath, false, directoryName);
                    if (template == null) continue;
                    templates.Add(template);
                }
            }

            directoryPath = GetTemplatesDirectoryPath();
            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                var directoryNames = DirectoryUtils.GetDirectoryNames(directoryPath);
                foreach (var directoryName in directoryNames)
                {
                    var template = GetFormTemplate(directoryPath, true, directoryName);
                    if (template == null) continue;
                    templates.Add(template);
                }
            }

            return templates;
        }

        public FormTemplate GetFormTemplate(Site site, string name)
        {
            var directoryPath = GetTemplatesDirectoryPath(site);
            var template = GetFormTemplate(directoryPath, false, name);
            if (template != null)
            {
                return template;
            }

            directoryPath = GetTemplatesDirectoryPath();
            return GetFormTemplate(directoryPath, true, name);
        }

        private FormTemplate GetFormTemplate(string templatesDirectoryPath, bool isSystem, string name)
        {
            if (!FileUtils.IsFileExists(PathUtils.Combine(templatesDirectoryPath, name, MAIN_FILE_NAME)))
            {
                return null;
            }

            return new FormTemplate
            {
                IsSystem = isSystem,
                Name = name,
            };
        }

        public void Clone(Site site, bool isSystemOriginal, string nameOriginal, string name)
        {
            var directoryPathSite = GetTemplatesDirectoryPath(site);
            var directoryPathToClone = isSystemOriginal ? GetTemplatesDirectoryPath() : directoryPathSite;

            DirectoryUtils.Copy(PathUtils.Combine(directoryPathToClone, nameOriginal), PathUtils.Combine(directoryPathSite, name), true);
        }

        public void Clone(Site site, bool isSystemOriginal, string nameOriginal, string name, string templateHtml)
        {
            Clone(site, isSystemOriginal, nameOriginal, name);
            SetTemplateHtml(site, name, templateHtml);
        }
    }
}