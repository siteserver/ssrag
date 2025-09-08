using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SqlKata;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface ISiteLogRepository : IRepository
    {
        Task InsertAsync(SiteLog log);

        Task DeleteIfThresholdAsync();

        Task DeleteAllAsync();

        Task<int> GetCountAsync(List<int> siteIds, string logType, int adminId, string keyword, string dateFrom,
            string dateTo);

        Task<List<SiteLog>> GetAllAsync(List<int> siteIds, string logType, int adminId, string keyword, string dateFrom,
            string dateTo, int offset, int limit);

        Query GetQuery(List<int> siteIds, string logType, int adminId, string keyword, string dateFrom, string dateTo);
    }
}
