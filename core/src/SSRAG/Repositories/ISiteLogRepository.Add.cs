using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface ISiteLogRepository
    {
        Task AddSiteLogAsync(int siteId, int channelId, int contentId, Administrator adminInfo, string ipAddress,
            string action, string summary);

        Task AddSiteCreateLogAsync(int siteId, int channelId, int contentId, Administrator adminInfo, string ipAddress, string filePath); 
    }
}
