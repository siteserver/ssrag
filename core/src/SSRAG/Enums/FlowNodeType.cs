using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FlowNodeType
    {
        // Chatflow
        [DataEnum(DisplayName = "输入")] Input,
        [DataEnum(DisplayName = "输出")] Output,
        [DataEnum(DisplayName = "知识库")] Dataset,
        [DataEnum(DisplayName = "大模型")] LLM,
        [DataEnum(DisplayName = "意图识别")] Intent,
        [DataEnum(DisplayName = "提问")] Ask,
        [DataEnum(DisplayName = "网站搜索")] Website,
        [DataEnum(DisplayName = "Bing 搜索")] Bing,
        [DataEnum(DisplayName = "文本处理")] Text,
        [DataEnum(DisplayName = "SQL 查询")] Sql,

        // General
        [DataEnum(DisplayName = "HTTP 调用")] Http,
        [DataEnum(DisplayName = "流程结束")] End,
    }
}
