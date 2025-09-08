using System.Threading.Tasks;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Plugins;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Enums;

namespace SSRAG.Cli.Jobs
{
    public class ThemePublishJob : IJobService
    {
        public string CommandName => "theme publish";

        private SiteType _siteType;
        private string _directory;
        private bool _isHelp;
        private readonly OptionSet _options;

        private readonly IPathManager _pathManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICliApiService _cliApiService;

        public ThemePublishJob(IPathManager pathManager, ISettingsManager settingsManager, IDatabaseManager databaseManager, ICliApiService cliApiService)
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
            _cliApiService = cliApiService;
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: ssrag {CommandName}");
            await console.WriteLineAsync("Summary: publish theme to marketplace");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/theme-publish.html");
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

            var (success, name, filePath) = await ThemePackageJob.PackageAsync(console, _pathManager, _settingsManager, _databaseManager,
                _siteType, _directory, false);

            if (!success) return;

            var fileSize = FileUtils.GetFileSizeByFilePath(filePath);

            await console.WriteLineAsync($"Theme Packaged: {filePath}");
            await console.WriteLineAsync($"Publishing theme {name} ({fileSize})...");

            (success, failureMessage) = await _cliApiService.ThemePublishAsync(filePath);
            if (success)
            {
                await console.WriteSuccessAsync($"Theme published, your theme will live at {CloudUtils.Www.GetThemeUrl(status.UserName, name)}.");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}
