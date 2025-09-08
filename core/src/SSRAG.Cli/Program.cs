﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSRAG.Cli.Core;
using SSRAG.Cli.Extensions;
using SSRAG.Core.Extensions;
using SSRAG.Core.Plugins.Extensions;
using SSRAG.Utils;
using Serilog;
using SSRAG.Cli.Abstractions;
using SSRAG.Configuration;

namespace SSRAG.Cli
{
    public static class Program
    {
        public static IApplication Application { get; private set; }

        static async Task Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.GetEncoding(936);
            }
            catch
            {
                try
                {
                    Console.OutputEncoding = Encoding.UTF8;
                }
                catch
                {
                    // ignored
                }
            }

            var contentRootPath = Directory.GetCurrentDirectory();

            var profilePath = CliUtils.GetOsUserConfigFilePath();
            var ssragPath = PathUtils.Combine(contentRootPath, Constants.ConfigFileName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile(profilePath, optional: true, reloadOnChange: true)
                .AddJsonFile(ssragPath, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/cli/log.log", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .CreateLogger();

            var services = new ServiceCollection();

            var entryAssembly = Assembly.GetExecutingAssembly();
            var assemblies = new List<Assembly> { entryAssembly }.Concat(entryAssembly.GetReferencedAssemblies().Select(Assembly.Load));

            var settingsManager = services.AddSettingsManager(configuration, contentRootPath, PathUtils.Combine(contentRootPath, Constants.WwwrootDirectory), entryAssembly);
            services.AddPlugins(configuration, settingsManager);
            //services.AddPluginServices(pluginManager);

            Application = new Application(settingsManager);
            var pluginManager = services.AddPlugins(configuration, settingsManager);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddRepositories(assemblies);
            services.AddPseudoServices();
            services.AddServices();
            services.AddCliServices();
            services.AddCliJobs();
            services.AddPluginServices(pluginManager);
            // services.AddTaskQueue();

            await Application.RunAsync(args);
        }
    }
}
