using System;

namespace SSRAG.Core.StlParser.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StlAttributeAttribute : Attribute
    {
        public string Title { get; set; }
    }
}
