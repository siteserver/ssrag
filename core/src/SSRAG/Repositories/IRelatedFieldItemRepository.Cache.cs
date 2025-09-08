using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IRelatedFieldItemRepository
    {
        Task<List<RelatedFieldItem>> GetRelatedFieldItemsAsync(int siteId, int relatedFieldId, int parentId);

        Task<RelatedFieldItem> GetAsync(int siteId, int id);

        Task<string> GetValueAsync(int siteId, int id);

        Task<List<Cascade<int>>> GetCascadesAsync(int siteId, int relatedFieldId, int parentId);
    }
}