using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_site_permissions")]
    public class SitePermissions : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn(Text = true)]
        public List<int> ChannelIds { get; set; }

        [DataColumn(Text = true)]
        public List<string> Permissions { get; set; }

        [DataColumn(Text = true)]
        public List<string> ContentPermissions { get; set; }
    }
}