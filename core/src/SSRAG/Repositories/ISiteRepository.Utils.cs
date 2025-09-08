using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Enums;

namespace SSRAG.Repositories
{
    public partial interface ISiteRepository
    {
        Task<List<int>> GetLatestSiteIdsAsync(List<int> siteIdsLatestAccessed,
            List<int> siteIdsWithPermissions);

        string GetSiteTypeIconClass(SiteType siteType);
    }
}
