﻿using Microsoft.Extensions.DependencyInjection;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Jobs;
using SSRAG.Cli.Services;
using SSRAG.Core.Services;
using SSRAG.Services;

namespace SSRAG.Cli.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCliServices(this IServiceCollection services)
        {
            services.AddSingleton<ITaskManager, TaskManager>();
            services.AddScoped<ICliApiService, CliApiService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IDataUpdateService, DataUpdateService>();
        }

        public static void AddCliJobs(this IServiceCollection services)
        {
            services.AddScoped<IJobService, BuildJob>();
            services.AddScoped<IJobService, DataBackupJob>();
            services.AddScoped<IJobService, DataRestoreJob>();
            services.AddScoped<IJobService, DataSyncJob>();
            services.AddScoped<IJobService, DataUpdateJob>();
            services.AddScoped<IJobService, InstallDownloadJob>();
            services.AddScoped<IJobService, InstallDatabaseJob>();
            services.AddScoped<IJobService, LoginJob>();
            services.AddScoped<IJobService, LogoutJob>();
            services.AddScoped<IJobService, PluginNewJob>();
            services.AddScoped<IJobService, PluginPackageJob>();
            services.AddScoped<IJobService, PluginPublishJob>();
            services.AddScoped<IJobService, PluginUnPublishJob>();
            services.AddScoped<IJobService, RegisterJob>();
            services.AddScoped<IJobService, RunJob>();
            services.AddScoped<IJobService, StatusJob>();
            services.AddScoped<IJobService, ThemePackageJob>();
            services.AddScoped<IJobService, ThemePublishJob>();
            services.AddScoped<IJobService, ThemeUnPublishJob>();
            services.AddScoped<IJobService, UpdateJob>();
        }
    }
}