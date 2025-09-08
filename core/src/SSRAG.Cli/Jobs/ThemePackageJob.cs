using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Cli.Models;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Core.Utils.Serialization;
using SSRAG.Plugins;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Dto;
using SSRAG.Enums;

namespace SSRAG.Cli.Jobs
{
    public class ThemePackageJob : IJobService
    {
        public string CommandName => "theme package";

        private SiteType _siteType;
        private string _directory;
        private bool _isHelp;
        private readonly OptionSet _options;

        private readonly IPathManager _pathManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;

        public ThemePackageJob(IPathManager pathManager, ISettingsManager settingsManager, IDatabaseManager databaseManager)
        {
            _options = new OptionSet
            {
                { "t|siteType=", "site type",
                    v => _siteType = TranslateUtils.ToEnum(v, SiteType.Web) },
                { "d|directory=", "site directory path",
                    v => _directory = v },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };

            _pathManager = pathManager;
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: ssrag {CommandName}");
            await console.WriteLineAsync("Summary: package theme to zip file");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/theme-package.html");
            await console.WriteLineAsync("Options:");
            _options.WriteOptionDescriptions(console.Out);
            await console.WriteLineAsync();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            using var console = new ConsoleUtils(false);
            if (_isHelp)
            {
                await WriteUsageAsync(console);
                return;
            }

            var (success, _, filePath) =
                await PackageAsync(console, _pathManager, _settingsManager, _databaseManager, _siteType, _directory, true);
            if (success)
            {
                var fileSize = FileUtils.GetFileSizeByFilePath(filePath);
                await console.WriteSuccessAsync($"Theme packaged: {filePath} ({fileSize})");
            }
        }

        public static async Task<(bool Success, string name, string filePath)> PackageAsync(IConsoleUtils console, IPathManager pathManager, ISettingsManager settingsManager, IDatabaseManager databaseManager, SiteType siteType, string directory, bool isOverride)
        {
            var site = await databaseManager.SiteRepository.GetSiteByDirectoryAsync(directory);
            var sitePath = pathManager.GetSitePath(site);

            if (site == null || !DirectoryUtils.IsDirectoryExists(sitePath))
            {
                await console.WriteErrorAsync($@"Invalid site directory path: ""{directory}""");
                return (false, null, null);
            }

            var readme = string.Empty;
            Theme theme = null;

            var readmePath = PathUtils.Combine(sitePath, "README.md");
            if (FileUtils.IsFileExists(readmePath))
            {
                readme = FileUtils.ReadText(readmePath);
                var yaml = MarkdownUtils.GetYamlFrontMatter(readme);
                if (!string.IsNullOrEmpty(yaml))
                {
                    readme = MarkdownUtils.RemoveYamlFrontMatter(readme);
                    theme = YamlUtils.Deserialize<Theme>(yaml);
                }
            }

            var writeReadme = false;
            if (theme == null || string.IsNullOrEmpty(theme.Name) || string.IsNullOrEmpty(theme.CoverUrl))
            {
                writeReadme = true;
                theme = new Theme
                {
                    Name = console.GetString("name:"),
                    CoverUrl = console.GetString("cover image url:"),
                    Summary = console.GetString("repository url:"),
                    Tags = console.GetStringList("tags:"),
                    ThumbUrls = console.GetStringList("thumb image urls:"),
                    Compatibilities = console.GetStringList("compatibilities:"),
                    Price = console.GetYesNo("is free?") ? 0 : console.GetDecimal("price:"),
                };
            }

            if (writeReadme)
            {
                readme = @$"---
{YamlUtils.Serialize(theme)}
---

" + readme;
                FileUtils.WriteText(readmePath, readme);
            }

            var packageName = "T_" + theme.Name.Replace(" ", "_");
            var packagePath = pathManager.GetSiteTemplatesPath(siteType, packageName);
            var fileName = packageName + ".zip";
            var filePath = pathManager.GetSiteTemplatesPath(siteType, fileName);

            if (!isOverride && FileUtils.IsFileExists(filePath))
            {
                return (true, theme.Name, filePath);
            }

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(packagePath);

            await console.WriteLineAsync($"Theme name: {theme.Name}");
            await console.WriteLineAsync($"Theme folder: {packagePath}");
            await console.WriteLineAsync("Theme packaging...");

            var caching = new Caching(settingsManager);
            var manager = new SiteTemplateManager(pathManager, databaseManager, caching, siteType);

            if (manager.IsSiteTemplateDirectoryExists(packageName))
            {
                manager.DeleteSiteTemplate(packageName);
            }

            var directoryNames = DirectoryUtils.GetDirectoryNames(sitePath);

            var directories = new List<string>();
            var siteDirList = await databaseManager.SiteRepository.GetSiteDirsAsync();
            foreach (var directoryName in directoryNames)
            {
                var isSiteDirectory = false;
                if (site.Root)
                {
                    foreach (var siteDir in siteDirList)
                    {
                        if (StringUtils.EqualsIgnoreCase(siteDir, directoryName))
                        {
                            isSiteDirectory = true;
                        }
                    }
                }
                if (!isSiteDirectory && !pathManager.IsSystemDirectory(directoryName))
                {
                    directories.Add(directoryName);
                }
            }

            var files = DirectoryUtils.GetFileNames(sitePath);

            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);
            await exportObject.ExportFilesToSiteAsync(packagePath, true, directories, files, true);

            var siteContentDirectoryPath = pathManager.GetSiteTemplateMetadataPath(packagePath, DirectoryUtils.SiteFiles.Templates.SiteContent);

            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, true, true, new List<int>());

            await SiteTemplateManager.ExportSiteToSiteTemplateAsync(pathManager, databaseManager, caching, site, packageName);

            var siteTemplate = new SiteTemplate
            {
                SiteTemplateName = theme.Name,
                PicFileName = string.Empty,
                WebSiteUrl = string.Empty,
                Description = string.Empty
            };
            var xmlPath = pathManager.GetSiteTemplateMetadataPath(packagePath, DirectoryUtils.SiteFiles.Templates.FileMetadata);
            XmlUtils.SaveAsXml(siteTemplate, xmlPath);

            pathManager.CreateZip(filePath, packagePath);

            return (true, theme.Name, filePath);
        }
    }
}
