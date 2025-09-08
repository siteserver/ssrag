using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TextProcessType
    {
        [DataEnum(DisplayName = "文本拼接")]
        Joint,

        [DataEnum(DisplayName = "文本分隔")]
        Split,

        [DataEnum(DisplayName = "文本替换")]
        Replace
    }
}