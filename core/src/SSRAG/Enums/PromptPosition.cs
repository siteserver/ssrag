using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PromptPosition
    {
        [DataEnum(DisplayName = "功能提示")]
        Functions,

        [DataEnum(DisplayName = "输入框提示")]
        Input,
    }
}
