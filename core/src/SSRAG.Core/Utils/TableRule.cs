using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Core.Utils
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TableRule
    {
        Choose,
        HandWrite,
        Create
    }
}
