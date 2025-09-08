using System.Collections.Generic;
using SSRAG.Enums;

namespace SSRAG.Dto
{
    public class BadWord
    {
        public BadWordsType Type { get; set; }
        public string Message { get; set; }
        public List<string> Words { get; set; }
    }
}
