using System.Threading.Tasks;
using SSRAG.Parse;

namespace SSRAG.Plugins
{
    public interface IPluginParseAsync : IPluginExtension
    {
        string ElementName { get; }
        Task<string> ParseAsync(IParseStlContext context);
    }
}
