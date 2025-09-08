using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VariableDataType
    {
        [DataEnum(DisplayName = "字符串")] String,
        [DataEnum(DisplayName = "整数")] Integer,
        [DataEnum(DisplayName = "数字")] Number,
        [DataEnum(DisplayName = "布尔值")] Boolean,
        [DataEnum(DisplayName = "对象")] Object,
        [DataEnum(DisplayName = "字符串数组")] ArrayString,
        [DataEnum(DisplayName = "整数数组")] ArrayInteger,
        [DataEnum(DisplayName = "数字数组")] ArrayNumber,
        [DataEnum(DisplayName = "布尔值数组")] ArrayBoolean,
        [DataEnum(DisplayName = "对象数组")] ArrayObject,
    }
}
