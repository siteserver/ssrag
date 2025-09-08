using System.Linq;
using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Core.Repositories
{
    public partial class UserGroupRepository
    {
        public async Task<bool> IsExistsAsync(string groupName)
        {
            var list = await GetUserGroupsAsync(true);
            return list.Any(group => group.GroupName == groupName);
        }

        public async Task<int> GetGroupIdAsync(string groupName)
        {
            var list = await GetUserGroupsAsync(false);
            var item = list.FirstOrDefault(group => group.GroupName == groupName);
            return item == null ? 0 : item.Id;
        }

        public async Task<UserGroup> GetAsync(int groupId)
        {
            var list = await GetUserGroupsAsync(false);
            return list.FirstOrDefault(group => group.Id == groupId);
        }

        public async Task<string> GetNameAsync(int groupId)
        {
            var group = await GetAsync(groupId);
            return group != null ? group.GroupName : string.Empty;
        }
    }
}
