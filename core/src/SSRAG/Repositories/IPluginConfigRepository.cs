using System.Threading.Tasks;
using SSRAG.Datory;

namespace SSRAG.Repositories
{
    public interface IPluginConfigRepository : IRepository
    {
        Task<T> GetAsync<T>(string pluginId, int siteId, string name);

        Task<T> GetAsync<T>(string pluginId, string name);

        Task<bool> ExistsAsync(string pluginId, int siteId, string name);

        Task<bool> ExistsAsync(string pluginId, string name);

        Task<bool> SetAsync<T>(string pluginId, int siteId, string name, T value);

        Task<bool> SetAsync<T>(string pluginId, string name, T value);

        Task<bool> RemoveAsync(string pluginId, int siteId, string name);

        Task<bool> RemoveAsync(string pluginId, string name);
    }
}