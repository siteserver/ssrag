using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Dto
{
    public class FlowNodeOutput : FlowNode
    {
        public FlowNodeOutput()
        {

        }

        public FlowNodeOutput(FlowNode node)
        {
            var dict = node.ToDictionary();
            LoadDict(dict);
        }

        public bool IsStreaming { get; set; }
        public string Output { get; set; }
    }
}
