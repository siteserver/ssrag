using SSRAG.Models;

namespace SSRAG.Dto
{
    public class FlowNodeBing : FlowNode
    {
        public FlowNodeBing()
        {

        }

        public FlowNodeBing(FlowNode node)
        {
            var dict = node.ToDictionary();
            LoadDict(dict);
        }

        public string BingKey { get; set; }
        public int BingCount { get; set; }
        public bool IsBingSite { get; set; }
        public string BingSiteUrl { get; set; }
    }
}
