using System.Collections.Generic;
using SSRAG.Models;

namespace SSRAG.Dto
{
    public class FlowNodeIntent : FlowNode
    {
        public FlowNodeIntent()
        {

        }

        public FlowNodeIntent(FlowNode node)
        {
            var dict = node.ToDictionary();
            LoadDict(dict);
        }

        public List<string> Intentions { get; set; }
        public int ChatHistories { get; set; }
        public int ChatModelId { get; set; }
        public string SystemPrompt { get; set; }
    }
}
