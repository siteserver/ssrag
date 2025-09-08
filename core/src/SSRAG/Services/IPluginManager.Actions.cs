using System.Threading.Tasks;

namespace SSRAG.Services
{
    public partial interface IPluginManager
    {
        Task InstallAsync(string userName, string name, string version, string downloadUrl);

        void UnInstall(string pluginId);
    }
}
