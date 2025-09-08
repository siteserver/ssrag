using System.Collections.Generic;
using System.Threading.Tasks;
using static SSRAG.Cli.Services.CliApiService;

namespace SSRAG.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, GetReleasesResult result, string failureMessage)> GetReleasesAsync(string version, List<string> pluginIds);
    }
}
