using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_site_log")]
    public class SiteLog : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public int AdminId { get; set; }

        [DataColumn]
        public string IpAddress { get; set; }

        [DataColumn]
        public string Action { get; set; }

        [DataColumn]
        public string Summary { get; set; }
    }
}
