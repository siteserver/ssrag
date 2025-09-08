using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IUserGroupRepository
    {
        Task<bool> IsExistsAsync(string groupName);

        Task<int> GetGroupIdAsync(string groupName);

        Task<UserGroup> GetAsync(int groupId);

        Task<string> GetNameAsync(int groupId);
    }
}
