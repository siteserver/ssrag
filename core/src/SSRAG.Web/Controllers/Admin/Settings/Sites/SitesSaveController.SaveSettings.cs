using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesSaveController
    {
        [HttpPost, Route(RouteSettings)]
        public async Task<ActionResult<SaveSettingsResult>> SaveSettings([FromBody] SaveRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var caching = new Caching(_settingsManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching, site.SiteType);

            if (manager.IsSiteTemplateDirectoryExists(request.TemplateDir))
            {
                return this.Error("知识库模板文件夹已存在，请更换知识库模板文件夹！");
            }

            var sitePath = _pathManager.GetSitePath(site);
            var directoryNames = DirectoryUtils.GetDirectoryNames(sitePath);

            var directories = new List<string>();
            var siteDirList = await _siteRepository.GetSiteDirsAsync();
            foreach (var directoryName in directoryNames)
            {
                var isSiteDirectory = false;
                if (site.Root)
                {
                    foreach (var siteDir in siteDirList)
                    {
                        if (StringUtils.EqualsIgnoreCase(siteDir, directoryName))
                        {
                            isSiteDirectory = true;
                        }
                    }
                }
                if (!isSiteDirectory && !_pathManager.IsSystemDirectory(directoryName))
                {
                    directories.Add(directoryName);
                }
            }

            var files = DirectoryUtils.GetFileNames(sitePath);

            return new SaveSettingsResult
            {
                Directories = directories,
                Files = files
            };
        }
    }
}
