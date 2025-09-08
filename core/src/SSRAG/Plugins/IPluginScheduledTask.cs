using System.Threading.Tasks;

namespace SSRAG.Plugins
{
    public interface IPluginScheduledTask : IPluginExtension
    {
        string TaskType { get; }
        Task ExecuteAsync(string settings);
    }
}
