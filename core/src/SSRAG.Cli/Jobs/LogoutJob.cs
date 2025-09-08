using System.Threading.Tasks;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Cli.Models;
using SSRAG.Plugins;
using SSRAG.Utils;
using SSRAG.Configuration;

namespace SSRAG.Cli.Jobs
{
    public class LogoutJob : IJobService
    {
        public string CommandName => "logout";

        private bool _isHelp;

        private readonly IConfigService _configService;
        private readonly OptionSet _options;

        public LogoutJob(IConfigService configService)
        {
            _configService = configService;
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
            await console.WriteLineAsync("Summary: user logout");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/logout.html");
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

            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                await console.WriteErrorAsync("you have not logged in");
                return;
            }

            status = new ConfigStatus
            {
                UserName = string.Empty,
                AccessToken = string.Empty
            };

            await _configService.SaveStatusAsync(status);

            await console.WriteSuccessAsync("you have successful logged out");
        }
    }
}