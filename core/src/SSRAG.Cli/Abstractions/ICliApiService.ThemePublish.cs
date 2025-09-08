using System.Threading.Tasks;

namespace SSRAG.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, string failureMessage)> ThemePublishAsync(string zipPath);
    }
}
