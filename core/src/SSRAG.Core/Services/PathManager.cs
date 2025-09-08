using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class PathManager : IPathManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITemplateLogRepository _templateLogRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public PathManager(ISettingsManager settingsManager, IPluginManager pluginManager, IDatabaseManager databaseManager, ISpecialRepository specialRepository, ITemplateLogRepository templateLogRepository, ITemplateRepository templateRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITableStyleRepository tableStyleRepository)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _databaseManager = databaseManager;
            _specialRepository = specialRepository;
            _templateLogRepository = templateLogRepository;
            _templateRepository = templateRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        //public string ApplicationPath => StringUtils.TrimEnd(_httpContext.Request.PathBase.Value, Constants.ApiPrefix);

        public string ContentRootPath => _settingsManager.ContentRootPath;
        public string WebRootPath => _settingsManager.WebRootPath;

        public string GetAdminUrl(params string[] paths)
        {
            return PageUtils.Combine($"/{Constants.AdminDirectory}", PageUtils.Combine(paths), "/");
        }

        public string GetHomeUrl(params string[] paths)
        {
            return PageUtils.Combine($"/{Constants.HomeDirectory}", PageUtils.Combine(paths));
        }

        //public string GetApiUrl(Site site, params string[] paths)
        //{
        //    return GetApiHostUrl(site, Constants.ApiPrefix, PageUtils.Combine(paths));
        //}

        public string GetApiHostUrl(Site site, params string[] paths)
        {
            var url = site.IsSeparatedApi ? site.SeparatedApiUrl : PageUtils.Separator;
            return PageUtils.Combine(url, PageUtils.Combine(paths));
        }

        public string GetUploadFileName(string fileName)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{StringUtils.GetRandomInt(1, 9999)}{PathUtils.GetExtension(fileName)}";
        }

        public string GetWebUrl(Site site)
        {
            return site.IsSeparatedWeb ? site.SeparatedWebUrl : GetLocalSiteUrl(site);
        }

        public string GetAssetsUrl(Site site)
        {
            return site.IsSeparatedAssets
                ? site.SeparatedAssetsUrl
                : PageUtils.Combine(GetWebUrl(site), site.AssetsDir);
        }

        public async Task UploadAsync(IFormFile file, string filePath)
        {
            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public async Task UploadAsync(byte[] bytes, string filePath)
        {
            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await stream.WriteAsync(bytes);
        }

        public async Task<(bool success, string filePath, string errorMessage)> UploadImageAsync(Site site, IFormFile file)
        {
            var fileName = PathUtils.GetFileName(file.FileName);

            var extName = PathUtils.GetExtension(fileName);
            if (!IsImageExtensionAllowed(site, extName))
            {
                return (false, string.Empty, Constants.ErrorImageExtensionAllowed);
            }
            if (!IsImageSizeAllowed(site, file.Length))
            {
                return (false, string.Empty, Constants.ErrorImageSizeAllowed);
            }

            var localDirectoryPath = GetUploadDirectoryPath(site, UploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, GetUploadFileName(site, fileName));

            await UploadAsync(file, filePath);

            if (site.IsImageAutoResize)
            {
                ImageUtils.ResizeImageIfExceeding(filePath, site.ImageAutoResizeWidth);
            }

            AddWaterMark(site, filePath);

            return (true, filePath, string.Empty);
        }

        public async Task<(bool success, string filePath, string errorMessage)> UploadFileAsync(Site site, IFormFile file)
        {
            var fileName = PathUtils.GetFileName(file.FileName);

            var extName = PathUtils.GetExtension(fileName);
            if (!IsFileExtensionAllowed(site, extName))
            {
                return (false, string.Empty, Constants.ErrorFileExtensionAllowed);
            }
            if (!IsFileSizeAllowed(site, file.Length))
            {
                return (false, string.Empty, Constants.ErrorFileSizeAllowed);
            }

            var localDirectoryPath = GetUploadDirectoryPath(site, UploadType.File);
            var filePath = PathUtils.Combine(localDirectoryPath, GetUploadFileName(site, fileName));

            await UploadAsync(file, filePath);

            return (true, filePath, string.Empty);
        }
    }
}