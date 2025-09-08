using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface ITranslateRepository : IRepository
    {
        Task InsertAsync(Translate translate);

        Task DeleteAsync(int siteId, int channelId);

        Task DeleteAllAsync(int siteId);

        Task<List<Translate>> GetTranslatesAsync(int siteId, bool summary = false);

        Task<List<Translate>> GetTranslatesAsync(int siteId, int channelId, bool summary = false);

        Task<string> GetSummaryAsync(Translate translate);
    }
}