using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IAdministratorRepository
    {
        Task<Administrator> GetByAccountAsync(string account);

        Task<Administrator> GetByUserIdAsync(int userId);

        Task<Administrator> GetByUuidAsync(string uuid);

        Task<Administrator> GetByUserNameAsync(string userName);

        Task<Administrator> GetByMobileAsync(string mobile);

        Task<Administrator> GetByEmailAsync(string email);

        string GetUserUploadFileName(string filePath);

        Task<string> GetDisplayAsync(int userId);

        Task<string> GetDisplayAsync(string userName);

        string GetDisplay(Administrator admin);
    }
}
