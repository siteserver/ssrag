using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Services
{
    public partial interface IAuthManager
    {
        Task<string> AuthenticateAsync(Administrator administrator, User user, bool isPersistent);

        // Task<string> RefreshAdministratorTokenAsync(string accessToken);

        // string AuthenticateUser(User user, bool isPersistent);

        // Task<string> RefreshUserTokenAsync(string accessToken);
    }
}
