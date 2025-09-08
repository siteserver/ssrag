using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_flow_node")]
    public class FlowNode : Entity
    {
        public FlowNode()
        {

        }

        public FlowNode(Dictionary<string, object> dict)
        {
            LoadDict(dict);
        }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string ParentId { get; set; }

        [DataColumn]
        public FlowNodeType NodeType { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn]
        public List<int> DepartmentIds { get; set; }

        [DataColumn]
        public List<int> GroupIds { get; set; }

        [DataColumn]
        public List<string> UserNames { get; set; }

        [DataColumn]
        public List<string> PluginIds { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public bool IsFixed { get; set; }
        public bool IsIgnoreExceptions { get; set; }
        public OutputFormat OutputFormat { get; set; }
        public List<Department> Departments { get; set; }
        public List<UserGroup> Groups { get; set; }
        public List<User> Users { get; set; }
        public List<FlowVariable> InVariables { get; set; }
        public List<FlowVariable> OutVariables { get; set; }
    }
}
