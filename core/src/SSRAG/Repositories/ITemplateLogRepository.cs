using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface ITemplateLogRepository : IRepository
    {
        Task InsertAsync(TemplateLog log);

        Task<string> GetTemplateContentAsync(int logId);

        Task<List<KeyValuePair<int, string>>> GetLogIdWithNameListAsync(int siteId, int templateId);

        Task DeleteAsync(int logId);

        Task DeleteAllAsync(int siteId);
    }
}