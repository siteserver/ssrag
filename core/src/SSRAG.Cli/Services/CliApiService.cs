﻿using System.Collections.Generic;
using SSRAG.Cli.Abstractions;
using SSRAG.Core.Utils;
using SSRAG.Utils;

namespace SSRAG.Cli.Services
{
    public partial class CliApiService : ICliApiService
    {
        private const string RestUrlLogin = "/login";
        private const string RestUrlStatus = "/status";
        private const string RestUrlRegister = "/register";
        private const string RestUrlPluginPublish = "/plugin-publish";
        private const string RestUrlPluginUnPublish = "/plugin-unpublish";
        private const string RestUrlReleases = "/releases";
        private const string RestUrlThemePublish = "/theme-publish";
        private const string RestUrlThemeUnPublish = "/theme-unpublish";

        public static string GetCliUrl(string relatedUrl) => PageUtils.Combine(CloudUtils.CloudApiHost, "cli", relatedUrl);

        private readonly IConfigService _configService;

        public CliApiService(IConfigService configService)
        {
            _configService = configService;
        }

        public class StatusResult
        {
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
        }

        public class LoginRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public bool IsPersistent { get; set; }
        }

        public class LoginResult
        {
            public string UserName { get; set; }
            public string AccessToken { get; set; }
        }

        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class PluginUnPublishRequest
        {
            public string PluginId { get; set; }
        }

        public class ThemeUnPublishRequest
        {
            public string Name { get; set; }
        }

        public class PluginAndUser
        {
            public Dictionary<string, object> Plugin { get; set; }
            public Dictionary<string, object> User { get; set; }
        }

        public class GetReleasesRequest
        {
            public string Version { get; set; }
            public List<string> PluginIds { get; set; }
        }

        public class GetReleasesCms
        {
            public string Version { get; set; }
            public string Published { get; set; }
        }

        public class GetReleasesPlugin
        {
            public string PluginId { get; set; }
            public string Version { get; set; }
            public string Published { get; set; }
        }

        public class GetReleasesResult
        {
            public GetReleasesCms Cms { get; set; }
            public List<GetReleasesPlugin> Plugins { get; set; }
        }
    }
}
