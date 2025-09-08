using System.Collections.Specialized;
using SSRAG.Parse;
using SSRAG.Services;

namespace SSRAG.Core.Context
{
    public class PluginParseStlContext : PluginParseContext, IParseStlContext
    {
        public PluginParseStlContext(IParseManager parseManager, string stlOuterHtml, string stlInnerHtml, NameValueCollection stlAttributes) : base(parseManager)
        {
            StlOuterHtml = stlOuterHtml;
            StlInnerHtml = stlInnerHtml;
            StlAttributes = stlAttributes;
        }

        public string StlOuterHtml { get; }

        public string StlInnerHtml { get; }

        public NameValueCollection StlAttributes { get; }

        public bool IsStlEntity => ParseManager.ContextInfo.IsStlEntity;
    }
}
