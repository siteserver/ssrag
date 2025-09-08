﻿using SSRAG.Core.StlParser.Attributes;

namespace SSRAG.Core.StlParser.StlElement
{
    [StlElement(Title = "SQL查询语句", Description = "通过 stl:queryString 标签在模板中定义SQL查询语句")]
    public static class StlQueryString
    {
        public const string ElementName = "stl:queryString";
    }
}
