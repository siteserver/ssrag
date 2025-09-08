using System.Threading.Tasks;

namespace SSRAG.Repositories
{
    public partial interface IAdministratorRepository
    {
        Task AddUserToRolesAsync(string userName, string[] roleNames);

        Task AddUserToRoleAsync(string userName, string roleName);

        Task<string> GetRolesAsync(string userName);
    }
}
