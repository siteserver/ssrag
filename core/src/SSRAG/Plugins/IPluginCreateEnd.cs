using SSRAG.Parse;

namespace SSRAG.Plugins
{
    public interface IPluginCreateEnd : IPluginExtension
    {
        void Parse(IParseContext context);
    }
}
