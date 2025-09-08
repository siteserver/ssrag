using System.Threading.Tasks;
using SSRAG.Cli.Models;

namespace SSRAG.Cli.Abstractions
{
    public interface IConfigService
    {
        public ConfigStatus Status { get; }

        Task SaveStatusAsync(ConfigStatus status);
    }
}
