using SSRAG.Models;

namespace SSRAG.Dto
{
    public class FlowNodeLLM : FlowNode
    {
        public FlowNodeLLM()
        {

        }

        public FlowNodeLLM(FlowNode node)
        {
            var dict = node.ToDictionary();
            LoadDict(dict);
        }

        public int ChatHistories { get; set; }
        public int ChatModelId { get; set; }
        public string SystemPrompt { get; set; }
        public string UserPrompt { get; set; }
        public bool IsReply { get; set; }
    }
}
