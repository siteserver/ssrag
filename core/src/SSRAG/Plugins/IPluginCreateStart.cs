using SSRAG.Parse;

namespace SSRAG.Plugins
{
    public interface IPluginCreateStart : IPluginExtension
    {
        void Parse(IParseContext context);
    }
}
