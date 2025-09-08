using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IRelatedFieldItemRepository : IRepository
    {
        Task<int> InsertAsync(RelatedFieldItem info);

        Task<bool> UpdateAsync(RelatedFieldItem info);

        Task DeleteAsync(int siteId, int id);

        Task DeleteAllAsync(int siteId);

        Task UpdateTaxisToDownAsync(int siteId, int relatedFieldId, int id, int parentId);

        Task UpdateTaxisToUpAsync(int siteId, int relatedFieldId, int id, int parentId);
    }
}