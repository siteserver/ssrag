using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SSRAG.Configuration;
using SSRAG.Core.Services;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Tests
{
    public class UnitTestsFixture
    {
        public ISettingsManager SettingsManager { get; }

        public UnitTestsFixture()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);

            var contentRootPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(dirPath)));

            //var contentRootPath = AppDomain.CurrentDomain.BaseDirectory;

            var config = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile(Constants.ConfigFileName)
                .Build();

            SettingsManager = new SettingsManager(null, config, contentRootPath, PathUtils.Combine(contentRootPath, Constants.WwwrootDirectory), null);
        }
    }
}
