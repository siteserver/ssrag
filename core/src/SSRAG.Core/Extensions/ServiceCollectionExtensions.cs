using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SSRAG.Datory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SSRAG.Core.Services;
using SSRAG.Services;

namespace SSRAG.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ISettingsManager AddSettingsManager(this IServiceCollection services, IConfiguration configuration, string contentRootPath, string webRootPath, Assembly entryAssembly)
        {
            var settingsManager = new SettingsManager(services, configuration, contentRootPath, webRootPath, entryAssembly);
            services.TryAdd(ServiceDescriptor.Singleton<ISettingsManager>(settingsManager));

            return settingsManager;
        }

        public static void AddRepositories(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            var baseType = typeof(IRepository);

            var types = assemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => x.IsClass).ToArray();
            var interfaceTypes = types.Where(x => x.IsInterface).ToArray();
            foreach (var implementType in implementTypes)
            {
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implementType);
                }
            }
        }

        public static void AddTaskServices(this IServiceCollection services)
        {
            services.AddHostedService<ScheduledHostedService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<ITaskManager, TaskManager>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICloudManager, CloudManager>();
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IPathManager, PathManager>();
            services.AddScoped<ICreateManager, CreateManager>();
            services.AddScoped<IDatabaseManager, DatabaseManager>();
            services.AddScoped<IFormManager, FormManager>();
            services.AddScoped<IParseManager, ParseManager>();
        }

        public static void AddPseudoServices(this IServiceCollection services)
        {
            services.AddScoped<IMailManager, CloudManager>();
            services.AddScoped<ISmsManager, CloudManager>();
            services.AddScoped<IStorageManager, CloudManager>();
            services.AddScoped<IVodManager, CloudManager>();
        }
    }
}