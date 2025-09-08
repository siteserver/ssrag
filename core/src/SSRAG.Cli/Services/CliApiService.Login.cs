using System.Threading.Tasks;
using SSRAG.Cli.Models;
using SSRAG.Core.Plugins;
using SSRAG.Core.Utils;
using SSRAG.Utils;

namespace SSRAG.Cli.Services
{
    public partial class CliApiService
    {
        public async Task<(bool success, string failureMessage)> LoginAsync(string account, string password)
        {
            var url = GetCliUrl(RestUrlLogin);
            var (success, result, _) = await RestUtils.PostAsync<LoginRequest, LoginResult>(url,
                new LoginRequest
                {
                    Account = account,
                    Password = AuthUtils.Md5ByString(password),
                    IsPersistent = true
                });

            if (!success)
            {
                return (false, "your account or password was incorrect");
            }

            var status = new ConfigStatus
            {
                UserName = result.UserName,
                AccessToken = result.AccessToken
            };

            await _configService.SaveStatusAsync(status);

            return (true, null);
        }
    }
}
