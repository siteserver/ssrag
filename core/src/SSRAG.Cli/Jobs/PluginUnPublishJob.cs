﻿using System.Threading.Tasks;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Configuration;
using SSRAG.Plugins;
using SSRAG.Utils;

namespace SSRAG.Cli.Jobs
{
    public class PluginUnPublishJob : IJobService
    {
        public string CommandName => "plugin unpublish";

        private string _directory;
        private bool _isHelp;

        private readonly ICliApiService _cliApiService;
        private readonly OptionSet _options;

        public PluginUnPublishJob(ICliApiService cliApiService)
        {
            _cliApiService = cliApiService;
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
            await console.WriteLineAsync($"Usage: ssrag {CommandName} <pluginId>");
            await console.WriteLineAsync("Summary: unpublishes a plugin. Example plugin id: ssrag.hits");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/plugin-unpublish.html");
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

            if (string.IsNullOrEmpty(_directory))
            {
                await console.WriteErrorAsync("missing required pluginId");
                return;
            }

            var (status, failureMessage) = await _cliApiService.GetStatusAsync();
            if (status == null)
            {
                await console.WriteErrorAsync(failureMessage);
                return;
            }

            bool success;
            (success, failureMessage) = await _cliApiService.PluginUnPublishAsync(_directory);
            if (success)
            {
                await console.WriteSuccessAsync($"Plugin {_directory} unpublished.");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}
