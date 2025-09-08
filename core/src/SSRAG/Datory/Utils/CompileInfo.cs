using System;
using System.Collections.Generic;
using System.Text;
using SSRAG.Datory.Caching;

namespace SSRAG.Datory.Utils
{
    internal class CompileInfo
    {
        public string Sql { get; set; }
        public Dictionary<string, object> NamedBindings { get; set; }
        public CachingCondition Caching { get; set; }
    }
}
