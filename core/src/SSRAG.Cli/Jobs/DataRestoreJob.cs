﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using Mono.Options;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Plugins;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Cli.Jobs
{
    public class DataRestoreJob : IJobService
    {
        public string CommandName => "data restore";

        private string _config;
        private string _directory;
        private List<string> _includes;
        private List<string> _excludes;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly OptionSet _options;

        public DataRestoreJob(ISettingsManager settingsManager, IDatabaseManager databaseManager)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;

            _options = new OptionSet
            {
                {
                    "c|config=", "Specify the file name of ssrag.json configuration file that you want to backup",
                    v => _config = v
                },
                {
                    "d|directory=", "Restore folder name",
                    v => _directory = v
                },
                {
                    "includes=", "Include table names, separated by commas, default restore all tables",
                    v => _includes = v == null ? null : ListUtils.GetStringList(v)
                },
                {
                    "excludes=", "Exclude table names, separated by commas",
                    v => _excludes = v == null ? null : ListUtils.GetStringList(v)
                },
                {
                    "h|help",  "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: ssrag {CommandName}");
            await console.WriteLineAsync("Summary: restore backup files to database");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/cli/commands/data-restore.html");
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
                await console.WriteErrorAsync("Restore folder name not specified: --directory");
                return;
            }

            var tree = new Tree(_settingsManager, _directory);

            if (!DirectoryUtils.IsDirectoryExists(tree.DirectoryPath))
            {
                await console.WriteErrorAsync($"恢复数据的文件夹 {tree.DirectoryPath} 不存在");
                return;
            }

            var tablesFilePath = tree.TablesFilePath;
            if (!FileUtils.IsFileExists(tablesFilePath))
            {
                await console.WriteErrorAsync($"恢复文件 {tree.TablesFilePath} 不存在");
                return;
            }

            var config = _config;
            if (string.IsNullOrEmpty(config))
            {
                config = Constants.ConfigFileName;
            }
            var configPath = PathUtils.Combine(_settingsManager.ContentRootPath, config);
            if (!FileUtils.IsFileExists(configPath))
            {
                await console.WriteErrorAsync($"The ssrag.json file does not exist: {configPath}");
                return;
            }
            _settingsManager.ChangeDatabase(configPath);

            await console.WriteLineAsync($"Database type: {_settingsManager.Database.DatabaseType.GetDisplayName()}");
            await console.WriteLineAsync($"Database connection string: {_settingsManager.Database.ConnectionString}");
            await console.WriteLineAsync($"Restore folder: {tree.DirectoryPath}");

            var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await console.WriteErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                return;
            }

            //if (!_dataOnly)
            //{
            //    if (!await _configRepository.IsNeedInstallAsync())
            //    {
            //        await console.WriteErrorAsync("The data could not be restored on the installed ssrag database");
            //        return;
            //    }

            //    // 恢复前先创建表，确保系统在恢复的数据库中能够使用
            //    //await _databaseManager.CreateSiteServerTablesAsync();

            //    if (_settingsManager.DatabaseType == DatabaseType.SQLite)
            //    {
            //        var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.DefaultLocalDbFileName);
            //        if (!FileUtils.IsFileExists(filePath))
            //        {
            //            await FileUtils.WriteTextAsync(filePath, string.Empty);
            //        }
            //    }

            //    await _databaseManager.SyncDatabaseAsync();
            //}

            await console.WriteRowLineAsync();
            await console.WriteRowAsync("Restore table name", "Count");
            await console.WriteRowLineAsync();

            var errorLogFilePath = CliUtils.DeleteErrorLogFileIfExists(_settingsManager);
            var errorTableNames = await _databaseManager.RestoreAsync(console, _includes, _excludes, tablesFilePath, tree, errorLogFilePath);

            if (errorTableNames.Count == 0)
            {
                await console.WriteSuccessAsync("restore database successfully!");
            }
            else
            {
                await console.WriteErrorAsync($"Database restore failed and the following table was not successfully restored: {ListUtils.ToString(errorTableNames)}");
            }
        }
    }
}
