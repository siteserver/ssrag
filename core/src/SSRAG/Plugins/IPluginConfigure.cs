using Microsoft.AspNetCore.Builder;

namespace SSRAG.Plugins
{
    public interface IPluginConfigure : IPluginExtension
    {
        void Configure(IApplicationBuilder app);
    }
}
