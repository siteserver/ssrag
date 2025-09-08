using Microsoft.Extensions.DependencyInjection;

namespace SSRAG.Plugins
{
    public interface IPluginConfigureServices : IPluginExtension
    {
        void ConfigureServices(IServiceCollection services);
    }
}
