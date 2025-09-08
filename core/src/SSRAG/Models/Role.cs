using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_role")]
    public class Role : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public string CreatorUserName { get; set; }

        [DataColumn]
        public string Description { get; set; }
    }
}
