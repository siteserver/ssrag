using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SyncType
    {
        [DataEnum(DisplayName = "Images")]
        Images,
        [DataEnum(DisplayName = "Videos")]
        Videos,
        [DataEnum(DisplayName = "Files")]
        Files,
        [DataEnum(DisplayName = "Pages")]
        Pages,
    }
}
