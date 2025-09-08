using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SSRAG.Cli.Abstractions;
using SSRAG.Cli.Core;
using SSRAG.Cli.Models;
using SSRAG.Utils;

namespace SSRAG.Cli.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IConfiguration _config;

        public ConfigService(IConfiguration config)
        {
            _config = config;
        }

        public ConfigStatus Status => _config.GetSection(nameof(Status)).Get<ConfigStatus>();

        public async Task SaveStatusAsync(ConfigStatus status)
        {
            var configPath = CliUtils.GetOsUserConfigFilePath();

            var config = new Config
            {
                Status = status
            };

            await FileUtils.WriteTextAsync(configPath, TranslateUtils.JsonSerialize(config));
        }
    }
}
