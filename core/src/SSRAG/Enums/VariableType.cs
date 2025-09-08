using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VariableType
    {
        [DataEnum(DisplayName = "输入")] Input,
        [DataEnum(DisplayName = "输出")] Output,
        [DataEnum(DisplayName = "全局")] Global,
    }
}
