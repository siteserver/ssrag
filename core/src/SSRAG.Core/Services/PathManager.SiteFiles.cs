using System.Threading.Tasks;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Enums;
using SSRAG.Datory;

namespace SSRAG.Core.Services
{
    public partial class PathManager
    {
        public string GetTemporaryFilesUrl(params string[] paths)
        {
            return GetSiteFilesUrl(DirectoryUtils.SiteFiles.TemporaryFiles, PageUtils.Combine(paths));
        }

        public string GetSiteTemplatesUrl(SiteType siteType, string relatedUrl)
        {
            return GetRootUrl(DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.Templates.DirectoryName, siteType.GetValue(), relatedUrl);
        }

        public string GetSiteFilesUrl(params string[] paths)
        {
            return GetRootUrl(DirectoryUtils.SiteFiles.DirectoryName, PageUtils.Combine(paths));
        }

        public string GetSiteFilesUrl(Site site, params string[] paths)
        {
            return site == null
                ? GetSiteFilesUrl(paths)
                : GetApiHostUrl(site, DirectoryUtils.SiteFiles.DirectoryName, PageUtils.Combine(paths));
        }

        public string GetSiteFilesPath(params string[] paths)
        {
            var path = PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName, PathUtils.Combine(paths));
            return path;
        }

        public async Task<string> GetMailTemplateHtmlAsync()
        {
            var htmlPath = GetSiteFilesPath("assets/mail/template.html");
            return await _settingsManager.Cache.GetByFilePathAsync(htmlPath);
        }

        public async Task<string> GetMailListHtmlAsync()
        {
            var htmlPath = GetSiteFilesPath("assets/mail/list.html");
            return await _settingsManager.Cache.GetByFilePathAsync(htmlPath);
        }
    }
}