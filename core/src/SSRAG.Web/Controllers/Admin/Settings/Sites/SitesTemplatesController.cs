using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Dto;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesTemplatesController : ControllerBase
    {
        private const string Route = "settings/sitesTemplates";
        private const string RouteDelete = "settings/sitesTemplates/actions/delete";
        private const string RouteZip = "settings/sitesTemplates/actions/zip";
        private const string RouteUnZip = "settings/sitesTemplates/actions/unZip";
        private const string RouteUpload = "settings/sitesTemplates/actions/upload";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;

        public SitesTemplatesController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
        }

        public class ListResult
        {
            public List<SiteTemplate> SiteTemplates { get; set; }
            public List<string> FileNameList { get; set; }
            public string SiteTemplateUrl { get; set; }
            public bool SiteAddPermission { get; set; }
        }

        public class ZipRequest
        {
            public SiteType SiteType { get; set; }
            public string DirectoryName { get; set; }
        }

        public class UnZipRequest
        {
            public SiteType SiteType { get; set; }
            public string FileName { get; set; }
        }

        public class DeleteRequest
        {
            public SiteType SiteType { get; set; }
            public string DirectoryName { get; set; }
            public string FileName { get; set; }
        }

        private async Task<ListResult> GetListResultAsync(SiteType siteType)
        {
            var caching = new Caching(_settingsManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching, siteType);
            var siteTemplates = manager.GetSiteTemplates();
            var siteTemplateInfoList = new List<SiteTemplate>();
            foreach (var siteTemplate in siteTemplates)
            {
                var directoryPath = _pathManager.GetSiteTemplatesPath(siteType, siteTemplate.DirectoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                if (string.IsNullOrEmpty(siteTemplate.SiteTemplateName)) continue;

                var filePath = _pathManager.GetSiteTemplatesPath(siteType, dirInfo.Name + ".zip");
                siteTemplate.FileExists = FileUtils.IsFileExists(filePath);
                siteTemplateInfoList.Add(siteTemplate);
            }

            var fileNames = manager.GetZipSiteTemplateList();
            var fileNameList = new List<string>();
            foreach (var fileName in fileNames)
            {
                if (DirectoryUtils.IsDirectoryExists(
                    _pathManager.GetSiteTemplatesPath(siteType, PathUtils.GetFileNameWithoutExtension(fileName)))) continue;
                var filePath = _pathManager.GetSiteTemplatesPath(siteType, fileName);
                var fileInfo = new FileInfo(filePath);
                fileNameList.Add(fileInfo.Name);
            }

            var siteTemplateUrl = StringUtils.TrimSlash(_pathManager.GetSiteTemplatesUrl(siteType, string.Empty));
            var siteAddPermission =
                await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd);

            return new ListResult
            {
                SiteTemplates = siteTemplateInfoList,
                FileNameList = fileNameList,
                SiteTemplateUrl = siteTemplateUrl,
                SiteAddPermission = siteAddPermission
            };
        }
    }
}
