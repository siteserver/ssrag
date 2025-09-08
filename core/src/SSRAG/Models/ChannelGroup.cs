using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_channel_group")]
    public class ChannelGroup : Entity
    {
        [DataColumn]
        public string GroupName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string Description { get; set; }
    }
}
