using SSRAG.Parse;

namespace SSRAG.Plugins
{
    public interface IPluginParse : IPluginExtension
    {
        string ElementName { get; }
        string Parse(IParseStlContext context);
    }
}
