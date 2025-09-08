using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PasswordFormat
    {
        [DataEnum(DisplayName = "不加密")] Clear,
        [DataEnum(DisplayName = "不可逆方式加密")] Hashed,
        [DataEnum(DisplayName = "可逆方式加密（DES）")] Encrypted,
        [DataEnum(DisplayName = "可逆方式加密（SM4）")] SM4,
    }
}