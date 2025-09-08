﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Plugins;

namespace SSRAG.Services
{
    public partial interface IPluginManager
    {
        string DirectoryPath { get; }
        List<IPlugin> Plugins { get; }
        List<IPlugin> EnabledPlugins { get; }
        List<IPlugin> NetCorePlugins { get; }
        List<IPlugin> GetPlugins(int siteId);
        List<IPlugin> GetPlugins(int siteId, int channelId);
        bool IsEnabled(string pluginId, int siteId);
        bool IsEnabled(string pluginId, int siteId, int channelId);
        IPlugin GetPlugin(string pluginId);
        void Load();
        IEnumerable<T> GetExtensions<T>(bool useCaching = true) where T : IPluginExtension;
        Task<Dictionary<string, object>> GetConfigAsync(string pluginId);
        Task SaveConfigAsync(string pluginId, Dictionary<string, object> config);
        (List<string> cssUrls, List<string> jsUrls) GetExternalUrls();
    }
}
