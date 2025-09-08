using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface ICeleryTaskRepository : IRepository
    {
        Task<List<CeleryTask>> GetAllAsync(int siteId);

        Task InsertAsync(CeleryTask task);

        Task UpdateAsync(CeleryTask task);

        Task DeleteAllAsync(int siteId);
    }
}
