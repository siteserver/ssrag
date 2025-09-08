using System.Threading.Tasks;
using SSRAG.Parse;

namespace SSRAG.Plugins
{
    public interface IPluginCreateEndAsync : IPluginExtension
    {
        Task ParseAsync(IParseContext context);
    }
}
