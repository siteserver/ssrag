using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IUserRepository
    {
        Task<List<int>> GetUserIdsAsync(int departmentId);
        
        Task UpdateDepartmentIdAsync(User user, int departmentId);

        Task SyncDepartmentCountAsync(int departmentId);
    }
}