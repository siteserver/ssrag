using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IContentRepository : IRepository
    {
        Task<Repository<Content>> GetRepositoryAsync(string tableName);
    }
}
