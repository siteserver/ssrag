using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_administrators_in_roles")]
    public class AdministratorsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public string UserName { get; set; }
    }
}
