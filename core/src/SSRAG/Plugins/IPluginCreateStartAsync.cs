using System.Threading.Tasks;
using SSRAG.Parse;

namespace SSRAG.Plugins
{
    public interface IPluginCreateStartAsync : IPluginExtension
    {
        Task ParseAsync(IParseContext context);
    }
}
