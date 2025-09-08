using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IPermissionsInRolesRepository : IRepository
    {
        Task InsertAsync(PermissionsInRoles pr);

        Task DeleteAsync(string roleName);

        Task<List<string>> GetAppPermissionsAsync(IEnumerable<string> roles);
    }
}
