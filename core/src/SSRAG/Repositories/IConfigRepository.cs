using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IConfigRepository : IRepository
    {
        Task<int> InsertAsync(Config config);

        Task UpdateAsync(Config config);

        Task<bool> IsInitializedAsync();

        Task UpdateConfigVersionAsync(string version);

        Task<bool> IsNeedInstallAsync();
    }
}
