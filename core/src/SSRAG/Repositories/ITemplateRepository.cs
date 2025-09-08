using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Services;

namespace SSRAG.Repositories
{
    public partial interface ITemplateRepository : IRepository
    {
        Task<int> InsertAsync(Template template);

        Task UpdateAsync(Template template);

        Task SetDefaultAsync(int templateId);

        Task DeleteAsync(IPathManager pathManager, Site site, int templateId);

        Task DeleteAllAsync(int siteId);

        Task CreateDefaultTemplateAsync(int siteId);
    }
}