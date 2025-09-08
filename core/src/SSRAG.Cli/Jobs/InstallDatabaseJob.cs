using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Datory.Utils;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Configuration;
using SSRAG.Plugins;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Cli.Jobs
{
    public class InstallDatabaseJob : IJobService
    {
        public string CommandName => "install database";

        private string _userName;
        private string _password;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly OptionSet _options;

        public InstallDatabaseJob(ISettingsManager settingsManager, IDatabaseManager databaseManager, IPathManager pathManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;

            _options = new OptionSet {
                { "u|userName",  "Super administrator username",
                    v => _userName = v },
                { "p|password",  "Super administrator password",
                    v => _password = v },
                { "h|help",  "Display help",
                    v => _isHelp = v != null }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: ssrag {CommandName}");
            await console.WriteLineAsync("Summary: install ssrag");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/install-database.html");
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

            if (!await _configRepository.IsNeedInstallAsync())
            {
                await console.WriteErrorAsync($"SSRAG has been installed in {_settingsManager.ContentRootPath}");
                return;
            }

            var userName = string.IsNullOrEmpty(_userName)
                ? console.GetString("Super administrator username:")
                : _userName;
            var password = string.IsNullOrEmpty(_password)
                ? console.GetPassword("Super administrator password:")
                : _password;

            var (valid, message) =
                await _administratorRepository.InsertValidateAsync(userName, password, string.Empty, string.Empty);
            if (!valid)
            {
                await console.WriteErrorAsync(message);
                return;
            }

            if (_settingsManager.Containerized)
            {
                if (_settingsManager.DatabaseType == DatabaseType.SQLite)
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath,
                        DbUtils.LocalDbContainerVirtualPath.Substring(1));
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        await FileUtils.WriteTextAsync(filePath, string.Empty);
                    }
                }
            }
            else
            {
                if (_settingsManager.DatabaseType == DatabaseType.SQLite)
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath,
                        DbUtils.LocalDbHostVirtualPath.Substring(1));
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        await FileUtils.WriteTextAsync(filePath, string.Empty);
                    }
                }
            }

            (valid, message) = await _databaseManager.InstallAsync(userName, password, string.Empty, string.Empty);
            if (!valid)
            {
                await console.WriteErrorAsync(message);
                return;
            }

            await FileUtils.WriteTextAsync(_pathManager.GetRootPath("index.html"), Constants.Html5Empty);

            await console.WriteSuccessAsync("Congratulations, SSRAG was installed successfully!");
        }
    }
}
