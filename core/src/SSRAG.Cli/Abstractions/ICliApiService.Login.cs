using System.Threading.Tasks;

namespace SSRAG.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, string failureMessage)> LoginAsync(string account, string password);
    }
}
