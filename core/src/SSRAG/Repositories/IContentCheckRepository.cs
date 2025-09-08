using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IContentCheckRepository : IRepository
    {

        Task InsertAsync(ContentCheck check);

        Task DeleteAllAsync(int siteId);

        Task<List<ContentCheck>> GetCheckListAsync(int siteId, int channelId, int contentId);
    }
}