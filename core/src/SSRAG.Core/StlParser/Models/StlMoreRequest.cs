﻿using SSRAG.Datory;
using SSRAG.Enums;

namespace SSRAG.Core.StlParser.Models
{
    public class StlMoreRequest : Entity
    {
        public int SiteId { get; set; }
        public int PageChannelId { get; set; }
        public int PageContentId { get; set; }
        public TemplateType TemplateType { get; set; }
        public string Template { get; set; }
        public int Page { get; set; }
    }
}
