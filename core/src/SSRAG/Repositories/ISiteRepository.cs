using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface ISiteRepository : IRepository
    {
        Task<(int, string)> InsertSiteAsync(Channel channel, Site site, string adminName);

        Task DeleteAsync(int siteId);

        Task UpdateAsync(Site site);

        Task UpdateTableNameAsync(int siteId, string tableName);

        Task<List<KeyValuePair<int, Site>>> ParserGetSitesAsync(string siteName, string siteDir, int startNum,
            int totalNum, ScopeType scopeType, TaxisType taxisType);
    }
}
