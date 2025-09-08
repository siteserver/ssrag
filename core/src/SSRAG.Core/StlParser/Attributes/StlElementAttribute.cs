﻿using System;

namespace SSRAG.Core.StlParser.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StlElementAttribute : Attribute
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
