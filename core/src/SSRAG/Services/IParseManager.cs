﻿using System;
using System.Text;
using System.Threading.Tasks;
using SqlKata;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Parse;

namespace SSRAG.Services
{
    public partial interface IParseManager
    {
        ISettingsManager SettingsManager { get; }
        IPathManager PathManager { get; }
        IDatabaseManager DatabaseManager { get; }
        IFormManager FormManager { get; }
        ParsePage PageInfo { get; set; }
        ParseContext ContextInfo { get; set; }

        Task InitAsync(EditMode editMode, Site site, int pageChannelId, int pageContentId, Template template, int specialId);

        Task ParseAsync(StringBuilder contentBuilder, string filePath, bool isDynamic);

        Task<string> GetDynamicScriptAsync(string dynamicApiUrl, Dynamic dynamic);

        Task<string> ParseDynamicAsync(Dynamic dynamic, string template);

        Task<string> AddStlErrorLogAsync(string elementName, string stlContent, Exception ex);

        Task<Channel> GetChannelAsync();

        Task<Content> GetContentAsync();
    }
}
