﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesAssetsEditorController : ControllerBase
    {
        private const string Route = "cms/templates/templatesAssetsEditor";
        private const string RouteUpdate = "cms/templates/templatesAssetsEditor/actions/update";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public TemplatesAssetsEditorController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IPathManager pathManager,
            ISiteRepository siteRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetRequest
        {
            public int SiteId { get; set; }
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
            public string FileType { get; set; }
        }

        public class GetResult
        {
            public string TemplatesAssetsIncludeDir { get; set; }
            public string TemplatesAssetsJsDir { get; set; }
            public string TemplatesAssetsCssDir { get; set; }
            public string Path { get; set; }
            public string Content { get; set; }
        }

        public class ContentRequest
        {
            public int SiteId { get; set; }
            public string Path { get; set; }
            public string FileType { get; set; }
            public string Content { get; set; }
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
        }

        public class ContentResult
        {
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
        }

        private async Task<ActionResult<ContentResult>> SaveFile(ContentRequest request, Site site, bool isAdd)
        {
            var filePath = string.Empty;
            var requestPath = PathUtils.RemoveParentPath(request.Path);
            var requestDirectoryPath = request.DirectoryPath; // PathUtils.RemoveParentPath(request.DirectoryPath);
            var requestFileName = PathUtils.RemoveParentPath(request.FileName);

            if (StringUtils.EqualsIgnoreCase(request.FileType, "html"))
            {
                filePath = _pathManager.GetSitePath(site, site.TemplatesAssetsIncludeDir, requestPath + ".html");
            }
            else if (StringUtils.EqualsIgnoreCase(request.FileType, "css"))
            {
                filePath = _pathManager.GetSitePath(site, site.TemplatesAssetsCssDir, requestPath + ".css");
            }
            else if (StringUtils.EqualsIgnoreCase(request.FileType, "js"))
            {
                filePath = _pathManager.GetSitePath(site, site.TemplatesAssetsJsDir, requestPath + ".js");
            }
            else
            {
                return this.Error("文件保存失败，必须为Html/Css/Js文件！");
            }

            var fileInfo = new FileInfo(filePath);
            if (!_pathManager.IsInRootDirectory(fileInfo.FullName))
            {
                return this.Error("文件保存失败，路径不正确！");
            }

            var filePathToDelete = string.Empty;
            if (isAdd)
            {
                if (FileUtils.IsFileExists(filePath))
                {
                    return this.Error("文件新增失败，同名文件已存在！");
                }
            }
            else
            {
                var originalFilePath = _pathManager.GetSitePath(site, requestDirectoryPath, requestFileName);
                if (!StringUtils.EqualsIgnoreCase(originalFilePath, filePath))
                {
                    filePathToDelete = originalFilePath;
                    if (FileUtils.IsFileExists(filePath))
                    {
                        return this.Error("文件编辑失败，同名文件已存在！");
                    }
                }
            }

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            await FileUtils.WriteTextAsync(filePath, request.Content);
            if (!string.IsNullOrEmpty(filePathToDelete))
            {
                FileUtils.DeleteFileIfExists(filePathToDelete);
            }

            var fileName = PathUtils.GetFileName(filePath);
            var sitePath = _pathManager.GetSitePath(site);
            var directoryPath = StringUtils.ReplaceStartsWithIgnoreCase(filePath, sitePath, string.Empty);
            directoryPath = StringUtils.ReplaceEndsWithIgnoreCase(directoryPath, fileName, string.Empty);
            directoryPath = StringUtils.TrimSlash(directoryPath);

            return new ContentResult
            {
                DirectoryPath = directoryPath,
                FileName = fileName
            };
        }
    }
}
