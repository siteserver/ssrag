﻿using System;
using System.Threading.Tasks;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Configuration;
using SSRAG.Core.Plugins;
using SSRAG.Plugins;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Cli.Jobs
{
    public class PluginPackageJob : IJobService
    {
        public string CommandName => "plugin package";

        private string _directory;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly OptionSet _options;

        public PluginPackageJob(ISettingsManager settingsManager, IPathManager pathManager)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _options = new OptionSet
            {
                {
                    "d|directory=", "plugin folder name",
                    v => _directory = v
                },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: ssrag {CommandName}");
            await console.WriteLineAsync("Summary: package plugin to zip file");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/plugin-package.html");
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

            var pluginPath = string.IsNullOrEmpty(_directory)
                ? _settingsManager.ContentRootPath
                : PathUtils.Combine(_pathManager.GetPluginPath(_directory));

            var (plugin, errorMessage) = await PluginUtils.ValidateManifestAsync(pluginPath);
            if (plugin == null)
            {
                await console.WriteErrorAsync(errorMessage);
                return;
            }

            var zipPath = Package(_pathManager, plugin);
            var fileSize = FileUtils.GetFileSizeByFilePath(zipPath);

            await console.WriteSuccessAsync($"Packaged: {zipPath} ({fileSize})");
        }

        public static string Package(IPathManager pathManager, Plugin plugin)
        {
            var outputPath = PathUtils.Combine(plugin.ContentRootPath, plugin.Output);
            var packageId = PluginUtils.GetPackageId(plugin.Publisher, plugin.Name, plugin.Version);

            var publishPath = CliUtils.GetOsUserPluginsDirectoryPath(packageId);
            DirectoryUtils.DeleteDirectoryIfExists(publishPath);
            var zipPath = CliUtils.GetOsUserPluginsDirectoryPath($"{packageId}.zip");
            FileUtils.DeleteFileIfExists(zipPath);

            foreach (var directoryName in DirectoryUtils.GetDirectoryNames(outputPath))
            {
                if (//StringUtils.EqualsIgnoreCase(directoryName, "bin") ||
                    StringUtils.EqualsIgnoreCase(directoryName, "obj") ||
                    StringUtils.EqualsIgnoreCase(directoryName, "node_modules") ||
                    StringUtils.EqualsIgnoreCase(directoryName, ".git") ||
                    StringUtils.EqualsIgnoreCase(directoryName, ".vs") ||
                    StringUtils.EqualsIgnoreCase(directoryName, ".vscode")
                ) continue;

                DirectoryUtils.Copy(PathUtils.Combine(outputPath, directoryName), PathUtils.Combine(publishPath, directoryName));
            }

            foreach (var fileName in DirectoryUtils.GetFileNames(outputPath))
            {
                if (StringUtils.EqualsIgnoreCase(fileName, Constants.PluginConfigFileName) ||
                    StringUtils.EndsWithIgnoreCase(fileName, ".csproj") ||
                    StringUtils.EndsWithIgnoreCase(fileName, ".csproj.user") ||
                    StringUtils.EndsWithIgnoreCase(fileName, ".sln") ||
                    StringUtils.EndsWithIgnoreCase(fileName, ".DotSettings.user") ||
                    StringUtils.EndsWithIgnoreCase(fileName, ".csproj.user") ||
                    StringUtils.EndsWithIgnoreCase(fileName, ".csproj.user") ||
                    StringUtils.EndsWithIgnoreCase(fileName, ".cs") ||
                    StringUtils.EndsWithIgnoreCase(fileName, ".zip")
                ) continue;

                FileUtils.CopyFile(PathUtils.Combine(outputPath, fileName), PathUtils.Combine(publishPath, fileName));
            }

            if (!PathUtils.IsEquals(outputPath, plugin.ContentRootPath))
            {
                FileUtils.CopyFile(PathUtils.Combine(plugin.ContentRootPath, Constants.PackageFileName),
                    PathUtils.Combine(publishPath, Constants.PackageFileName));

                if (!FileUtils.IsFileExists(PathUtils.Combine(plugin.ContentRootPath, Constants.ReadmeFileName)))
                {
                    FileUtils.CopyFile(PathUtils.Combine(plugin.ContentRootPath, Constants.ReadmeFileName),
                        PathUtils.Combine(publishPath, Constants.ReadmeFileName));
                }
                if (!FileUtils.IsFileExists(PathUtils.Combine(plugin.ContentRootPath, Constants.ChangeLogFileName)))
                {
                    FileUtils.CopyFile(PathUtils.Combine(plugin.ContentRootPath, Constants.ChangeLogFileName),
                        PathUtils.Combine(publishPath, Constants.ChangeLogFileName));
                }
            }

            pathManager.CreateZip(zipPath, publishPath);
            DirectoryUtils.DeleteDirectoryIfExists(publishPath);

            return zipPath;
        }
    }
}
