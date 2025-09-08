using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_users_in_groups")]
    public class UsersInGroups : Entity
    {
        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public int GroupId { get; set; }
    }
}