using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_permissions_in_roles")]
    public class PermissionsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn(Text = true)]
        public List<string> AppPermissions { get; set; }
    }
}