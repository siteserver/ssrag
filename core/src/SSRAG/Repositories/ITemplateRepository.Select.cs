using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Models;
using SSRAG.Dto;

namespace SSRAG.Repositories
{
    public partial interface ITemplateRepository
    {
        Task<List<TemplateSummary>> GetSummariesAsync(int siteId);

        Task<Template> GetAsync(int templateId);
    }
}