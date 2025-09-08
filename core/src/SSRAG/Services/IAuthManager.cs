using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Services
{
    public partial interface IAuthManager
    {
        Task<User> GetUserAsync();

        Task<Administrator> GetAdminAsync();

        bool IsAdmin { get; }

        string AdminName { get; }

        bool IsUser { get; }

        string UserName { get; }

        bool IsApi { get; }

        string ApiToken { get; }

        string GetCSRFToken();
    }
}