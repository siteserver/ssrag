using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IUserMenuRepository : IRepository
    {
        Task<int> InsertAsync(UserMenu userMenu);

        Task UpdateAsync(UserMenu userMenu);

        Task DeleteAsync(int menuId);

        Task<List<UserMenu>> GetUserMenusAsync();

        Task<UserMenu> GetAsync(int id);

        Task ResetAsync();
    }
}