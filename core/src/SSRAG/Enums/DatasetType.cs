using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DatasetType
    {
        [DataEnum(DisplayName = "应用知识库")] Apps,
        [DataEnum(DisplayName = "站点知识库")] Sites,
        [DataEnum(DisplayName = "栏目知识库")] Channels,
    }
}
