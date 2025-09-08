using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IUserRepository
    {
        Task<User> GetByAccountAsync(string account);

        Task<User> GetByUserIdAsync(int userId);

        Task<User> GetByUserNameAsync(string userName);

        Task<User> GetByMobileAsync(string mobile);

        Task<User> GetByEmailAsync(string email);

        Task<User> GetByOpenIdAsync(string openId);

        Task<User> GetByUuidAsync(string uuid);

        Task<string> GetDisplayAsync(int userId);

        string GetDisplay(User user);
    }
}