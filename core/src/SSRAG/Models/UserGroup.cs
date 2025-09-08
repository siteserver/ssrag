using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_user_group")]
    public class UserGroup : Entity
    {
        [DataColumn]
        public string GroupName { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string AdminName { get; set; }

        [DataColumn]
        public string Description { get; set; }

        public bool IsDefault { get; set; }

        public bool IsManager { get; set; }
    }
}
