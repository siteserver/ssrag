using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_flow_edge")]
    public class FlowEdge : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string Source { get; set; }

        [DataColumn]
        public string SourceHandle { get; set; }

        [DataColumn]
        public string Target { get; set; }

        [DataColumn]
        public string TargetHandle { get; set; }
    }
}
