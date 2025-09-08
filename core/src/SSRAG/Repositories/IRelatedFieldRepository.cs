using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IRelatedFieldRepository : IRepository
    {
        Task<int> InsertAsync(RelatedField relatedField);

        Task<bool> UpdateAsync(RelatedField relatedField);

        Task DeleteAsync(int id);

        Task DeleteAllAsync(int siteId);

        Task<RelatedField> GetAsync(int siteId, string title);

        Task<RelatedField> GetAsync(int siteId, int relatedFieldId);

        Task<List<RelatedField>> GetRelatedFieldsAsync(int siteId);

        Task<string> GetImportTitleAsync(int siteId, string relatedFieldName);
    }
}