﻿using System.Threading.Tasks;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Configuration;
using SSRAG.Plugins;
using SSRAG.Utils;

namespace SSRAG.Cli.Jobs
{
    public class LoginJob : IJobService
    {
        public string CommandName => "login";

        private string _account;
        private string _password;
        private bool _isHelp;

        private readonly ICliApiService _cliApiService;
        private readonly OptionSet _options;

        public LoginJob(ICliApiService cliApiService)
        {
            _cliApiService = cliApiService;
            _options = new OptionSet
            {
                {
                    "a|account=", "Login account(username, mobile or email)",
                    v => _account = v
                },
                {
                    "p|password=", "Login password",
                    v => _password = v
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
            await console.WriteLineAsync("Summary: login to ssrag.com");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/login.html");
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

            if (string.IsNullOrEmpty(_account))
            {
                _account = console.GetString("Username:");
            }

            if (string.IsNullOrEmpty(_password))
            {
                _password = console.GetPassword("Password:");
            }

            var (success, failureMessage) = await _cliApiService.LoginAsync(_account, _password);
            if (success)
            {
                await console.WriteSuccessAsync("you have successful logged in");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}