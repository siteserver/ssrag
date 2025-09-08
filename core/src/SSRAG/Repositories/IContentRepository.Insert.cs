using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IContentRepository
    {
        Task<int> InsertAsync(Site site, Channel channel, Content content);

        Task<int> InsertPreviewAsync(Site site, Channel channel, Content content);

        Task<int> InsertWithTaxisAsync(Site site, Channel channel, Content content, int taxis);
    }
}
