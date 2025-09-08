using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Utils;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            if (_settingsManager.MaxSites > 0)
            {
                var siteIds = await _siteRepository.GetSiteIdsAsync();
                if (siteIds.Count >= _settingsManager.MaxSites)
                {
                    return this.Error("站点数量已超过限制，无法创建新站点!");
                }
            }

            var caching = new Caching(_settingsManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching, SiteType.Web);
            var siteTemplates = manager.GetSiteTemplates();

            var tableNameList = await _siteRepository.GetSiteTableNamesAsync();
            var rootExists = await _siteRepository.GetSiteByIsRootAsync() != null;
            var sites = await _siteRepository.GetSitesAsync();

            return new GetResult
            {
                SiteTemplates = siteTemplates,
                RootExists = rootExists,
                Sites = sites,
                TableNameList = tableNameList,
                Uuid = StringUtils.GetShortUuid(false)
            };
        }
    }
}