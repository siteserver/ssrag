using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OutputFormat
    {
        [DataEnum(DisplayName = "文本")] Text,
        [DataEnum(DisplayName = "Markdown")] Markdown,
        [DataEnum(DisplayName = "JSON")] JSON,
    }
}
