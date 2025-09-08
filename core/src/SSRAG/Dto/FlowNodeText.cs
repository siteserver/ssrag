using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Dto
{
    public class FlowNodeText : FlowNode
    {
        public FlowNodeText()
        {

        }

        public FlowNodeText(FlowNode node)
        {
            var dict = node.ToDictionary();
            LoadDict(dict);
        }

        public TextProcessType TextProcessType { get; set; }
        public string TextJoint { get; set; }
        public string TextReplace { get; set; }
        public bool IsTextCaseIgnore { get; set; }
        public bool IsTextRegex { get; set; }
        public string TextTo { get; set; }
        public string TextSplit { get; set; }
    }
}
