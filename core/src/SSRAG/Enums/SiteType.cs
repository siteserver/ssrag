using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SiteType
    {
        [DataEnum(DisplayName = "网站知识库")]
        Web,
        [DataEnum(DisplayName = "Markdown知识库")]
        Markdown,
        [DataEnum(DisplayName = "文档知识库")]
        Document,
        [DataEnum(DisplayName = "对话应用")]
        Chat,
        [DataEnum(DisplayName = "工作流应用")]
        Chatflow,
        [DataEnum(DisplayName = "智能体应用")]
        Agent,
    }
}
