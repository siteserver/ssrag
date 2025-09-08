﻿using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Services
{
    partial interface IPathManager
    {
        string GetTemporaryFilesUrl(params string[] paths);
        
        string GetSiteFilesPath(params string[] paths);

        string GetSiteFilesUrl(params string[] paths);

        string GetSiteFilesUrl(Site site, params string[] paths);

        Task<string> GetMailTemplateHtmlAsync();

        Task<string> GetMailListHtmlAsync();
    }
}
