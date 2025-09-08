using System;
using SSRAG.Datory;
using SSRAG.Utils;

namespace SSRAG.Dto
{
    [Serializable]
    public class Option<T>
    {
        public Option()
        {

        }

        public Option(T value, string label)
        {
            Value = value;
            Label = label;
        }

        public Option(Enum e)
        {
            Value = TranslateUtils.Get<T>(e.GetValue());
            Label = e.GetDisplayName();
        }

        public T Value { get; set; }
        public string Label { get; set; }
        public bool Selected { get; set; }
    }
}
