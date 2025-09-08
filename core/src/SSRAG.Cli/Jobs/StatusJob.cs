﻿using System.Reflection;
using System.Threading.Tasks;
using SSRAG.Datory;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Plugins;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Cli.Jobs
{
    public class StatusJob : IJobService
    {
        public string CommandName => "status";

        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly ICliApiService _cliApiService;
        private readonly OptionSet _options;

        public StatusJob(ISettingsManager settingsManager, IPluginManager pluginManager, ICliApiService cliApiService)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _cliApiService = cliApiService;
            _options = new OptionSet
            {
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: ssrag {CommandName}");
            await console.WriteLineAsync("Summary: show status");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/status.html");
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

            await console.WriteLineAsync($"Cli version: {_settingsManager.Version}");
            var entryAssembly = Assembly.GetExecutingAssembly();
            await console.WriteLineAsync($"Cli location: {entryAssembly.Location}");
            await console.WriteLineAsync($"Work location: {_settingsManager.ContentRootPath}");
            await console.WriteLineAsync($"Api host: {CloudUtils.CloudApiHost}");

            var configPath = CliUtils.GetConfigPath(_settingsManager);
            if (FileUtils.IsFileExists(configPath))
            {
                await console.WriteLineAsync($"Database type: {_settingsManager.Database.DatabaseType.GetDisplayName()}");
                await console.WriteLineAsync($"Database connection string: {_settingsManager.DatabaseConnectionString}");

                await console.WriteLineAsync($"Database type encrypted: {EncryptUtils.Encrypt(_settingsManager.Database.DatabaseType.GetDisplayName(), _settingsManager.SecurityKey)}");
                await console.WriteLineAsync($"Database connection string encrypted: {EncryptUtils.Encrypt(_settingsManager.DatabaseConnectionString, _settingsManager.SecurityKey)}");

                if (!string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString))
                {
                    var (isConnectionWorks, errorMessage) =
                        await _settingsManager.Database.IsConnectionWorksAsync();

                    if (!isConnectionWorks)
                    {
                        await console.WriteErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                        return;
                    }

                    await console.WriteLineAsync("Database status: Connection successful");
                }

                var plugins = _pluginManager.Plugins;
                foreach (var plugin in plugins)
                {
                    await console.WriteLineAsync($"PluginId: {plugin.PluginId}, Version: {plugin.Version}");
                }
            }
            else
            {
                await console.WriteLineAsync($"The ssrag.json file does not exist: {configPath}");
            }

            var (status, _) = await _cliApiService.GetStatusAsync();
            if (status != null)
            {
                await console.WriteLineAsync($"Login user: {status.UserName}");
            }
        }
    }
}