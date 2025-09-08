using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface ISiteRepository
    {
        Task<List<Site>> GetSitesAsync();

        Task<List<Site>> GetSitesAsync(params SiteType[] siteTypes);

        Task<string> GetTableNameAsync(int siteId);

        Task<Site> GetSiteBySiteNameAsync(string siteName);

        Task<Site> GetSiteByIsRootAsync();

        Task<bool> IsRootExistsAsync();

        Task<Site> GetSiteByDirectoryAsync(string siteDir);

        Task<List<int>> GetSiteIdsAsync();

        Task<List<string>> GetSiteTableNamesAsync();

        Task<List<string>> GetAllTableNamesAsync();

        Task<List<string>> GetTableNamesAsync(Site site);

        Task<int> GetIdByIsRootAsync();

        Task<string> GetSiteDirCascadingAsync(int siteId);

        Task<IList<string>> GetSiteDirsAsync();

        Task<List<Select<int>>> GetSelectsAsync(List<int> includedSiteIds = null);

        Task ClearCacheAsync(int siteId);
    }
}