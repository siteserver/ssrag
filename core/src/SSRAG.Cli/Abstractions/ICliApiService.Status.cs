using System.Threading.Tasks;
using SSRAG.Cli.Models;

namespace SSRAG.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(ConfigStatus status, string failureMessage)> GetStatusAsync();
    }
}
