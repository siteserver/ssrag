using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IContentGroupRepository : IRepository
    {
        Task InsertAsync(ContentGroup group);

        Task UpdateAsync(ContentGroup group);

        Task DeleteAsync(int siteId, string groupName);

        Task DeleteAllAsync(int siteId);

        Task UpdateTaxisDownAsync(int siteId, int groupId, int taxis);

        Task UpdateTaxisUpAsync(int siteId, int groupId, int taxis);

        Task<ContentGroup> GetAsync(int siteId, int groupId);

        Task<List<ContentGroup>> GetContentGroupsAsync(int siteId);
    }
}