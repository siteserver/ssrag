using SSRAG.Enums;

namespace SSRAG.Configuration
{
    public class InputStyleRule
    {
        public ValidateType Type { get; set; }
        public string Value { get; set; }
        public string Message { get; set; }
    }
}