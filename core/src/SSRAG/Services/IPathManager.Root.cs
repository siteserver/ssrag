using System.Threading.Tasks;
using SSRAG.Models;
using SSRAG.Enums;

namespace SSRAG.Services
{
    public partial interface IPathManager
    {
        string GetRootUrl(params string[] paths);

        string GetRootUrlByPath(string physicalPath);

        string GetRootPath(params string[] paths);

        bool IsInRootDirectory(string filePath);

        string GetTemporaryFilesPath(params string[] paths);

        Task<string> WriteTemporaryTextAsync(string value);

        string GetSiteTemplatesUrl(SiteType siteType, string relatedUrl);

        string ParseUrl(string url);

        string ParsePath(string directoryPath, string virtualPath);

        string GetAdministratorUploadPath(string userName, params string[] paths);

        string GetAdministratorUploadUrl(string userName, params string[] paths);

        string GetUserUploadPath(string userName, params string[] paths);

        string GetUserUploadUrl(string userName, params string[] paths);

        string GetHomeUploadPath(params string[] paths);

        string GetHomeUploadUrl(params string[] paths);

        string DefaultAvatarUrl { get; }

        string GetUserUploadPath(string userName, string relatedPath);

        string GetUserUploadFileName(string filePath);

        string GetUserUploadUrl(string userName, string relatedUrl);

        string GetUserAvatarUrl(User user);

        string GetDownloadApiUrl(Site site, int channelId, int contentId, string fileUrl);

        string GetDownloadApiUrl(Site site, string fileUrl);

        string GetDownloadApiUrl(string filePath);

        string GetDynamicApiUrl(Site site);

        string GetIfApiUrl(Site site);

        string GetPageContentsApiUrl(Site site);

        string GetPageContentsApiParameters(int siteId, int pageChannelId, int templateId, int totalNum, int pageCount,
            int currentPageIndex, string stlPageContentsElement);

        string GetTriggerApiUrl(int siteId, int channelId, int contentId, int fileTemplateId, int specialId, bool isRedirect);
    }
}
