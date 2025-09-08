﻿using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace SSRAG.Tests.Utils
{
    public class TestDirectoryUtils
    {
        [Fact]
        public void TestGetParentPath()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);

            var binDirectoryPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(dirPath));
            Assert.Equal("Bin", PathUtils.GetDirectoryName(binDirectoryPath, false), StringComparer.OrdinalIgnoreCase);

            var testsDirectoryPath = DirectoryUtils.GetParentPath(binDirectoryPath, 2);
            Assert.Equal("tests", PathUtils.GetDirectoryName(testsDirectoryPath, false), StringComparer.OrdinalIgnoreCase);
        }
    }
}
