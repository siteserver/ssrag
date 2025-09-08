using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_flow_variable")]
    public class FlowVariable : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string NodeId { get; set; }

        [DataColumn]
        public VariableType Type { get; set; }

        [DataColumn]
        public string Name { get; set; }

        [DataColumn]
        public VariableDataType DataType { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn]
        public bool IsDisabled { get; set; }

        [DataColumn]
        public bool IsReference { get; set; }

        [DataColumn]
        public string ReferenceNodeId { get; set; }

        [DataColumn]
        public string ReferenceName { get; set; }

        [DataColumn]
        public string Value { get; set; }
    }
}