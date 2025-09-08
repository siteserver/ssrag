using System.Threading.Tasks;
using SSRAG.Plugins;
using SSRAG.Utils;

namespace SSRAG.Cli.Abstractions
{
    public interface IJobService
    {
        string CommandName { get; }
        Task WriteUsageAsync(IConsoleUtils console);
        Task ExecuteAsync(IPluginJobContext context);
    }
}
