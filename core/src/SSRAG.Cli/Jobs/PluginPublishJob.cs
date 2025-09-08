﻿using System.Threading.Tasks;
using Mono.Options;
using Semver;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Configuration;
using SSRAG.Core.Plugins;
using SSRAG.Core.Utils;
using SSRAG.Plugins;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Cli.Jobs
{
    public class PluginPublishJob : IJobService
    {
        public string CommandName => "plugin publish";

        private string _directory;
        private string _version;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ICliApiService _cliApiService;
        private readonly OptionSet _options;

        public PluginPublishJob(ISettingsManager settingsManager, IPathManager pathManager, ICliApiService cliApiService)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _cliApiService = cliApiService;
            _options = new OptionSet
            {
                {
                    "d|directory=", "plugin folder name",
                    v => _directory = v
                },
                { "v|version=", "plugin version",
                    v => _version = v },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: ssrag {CommandName}");
            await console.WriteLineAsync("Summary: publish plugin to marketplace");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/plugin-publish.html");
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

            var (status, failureMessage) = await _cliApiService.GetStatusAsync();
            if (status == null)
            {
                await console.WriteErrorAsync(failureMessage);
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

            if (!string.IsNullOrEmpty(_version))
            {
                SemVersion.TryParse(plugin.Version, out var pluginVersion);
                string versionChanged;

                if (_version == "major")
                {
                    versionChanged = pluginVersion.Change(pluginVersion.Major + 1).ToString();
                }
                else if (_version == "minor")
                {
                    versionChanged = pluginVersion.Change(pluginVersion.Major, pluginVersion.Minor + 1).ToString();
                }
                else if (_version == "patch")
                {
                    versionChanged = pluginVersion.Change(pluginVersion.Major, pluginVersion.Minor, pluginVersion.Patch + 1).ToString();
                }
                else if (PluginUtils.IsSemVersion(_version))
                {
                    versionChanged = _version;
                }
                else
                {
                    await console.WriteErrorAsync($"Invalid plugin version '{_version}'");
                    return;
                }

                if (versionChanged != plugin.Version)
                {
                    await PluginUtils.UpdateVersionAsync(pluginPath, versionChanged);
                    (plugin, errorMessage) = await PluginUtils.ValidateManifestAsync(pluginPath);
                    if (plugin == null)
                    {
                        await console.WriteErrorAsync(errorMessage);
                        return;
                    }
                }
            }

            var packageId = PluginUtils.GetPackageId(plugin.Publisher, plugin.Name, plugin.Version);
            var zipPath = PluginPackageJob.Package(_pathManager, plugin);
            var fileSize = FileUtils.GetFileSizeByFilePath(zipPath);

            await console.WriteLineAsync($"Packaged: {zipPath}");
            await console.WriteLineAsync($"Publishing {packageId} ({fileSize})...");

            bool success;
            (success, failureMessage) = await _cliApiService.PluginPublishAsync(plugin.Publisher, zipPath);
            if (success)
            {
                await console.WriteSuccessAsync($"Published {packageId}, your plugin will live at {CloudUtils.Www.GetPluginUrl(plugin.Publisher, plugin.Name)}.");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}
