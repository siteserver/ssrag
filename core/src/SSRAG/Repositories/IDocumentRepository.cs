using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IDocumentRepository : IRepository
    {
        Task<int> InsertAsync(Document document);

        Task<int> GetCountAsync(int siteId, int channelId);
    }
}