using System;
using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Configuration;
using SSRAG.Enums;

namespace SSRAG.Models
{
    public class Content : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public string AdminName { get; set; }

        [DataColumn]
        public string LastEditAdminName { get; set; }

        [DataColumn]
        public string UserName { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public List<string> GroupNames { get; set; }

        [DataColumn]
        public List<string> TagNames { get; set; }

        [DataColumn]
        public int SourceId { get; set; }

        [DataColumn]
        public int ReferenceId { get; set; }

        [DataColumn]
        public int TemplateId { get; set; }

        [DataColumn]
        public bool Checked { get; set; }

        [DataColumn]
        public int CheckedLevel { get; set; }

        [DataColumn]
        public int Hits { get; set; }

        [DataColumn]
        public int Downloads { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string SubTitle { get; set; }

        [DataColumn]
        public string ImageUrl { get; set; }

        [DataColumn]
        public string VideoUrl { get; set; }

        [DataColumn]
        public string FileUrl { get; set; }

        [DataColumn(Text = true)]
        public string Body { get; set; }

        [DataColumn(Text = true)]
        public string Summary { get; set; }

        [DataColumn]
        public string Author { get; set; }

        [DataColumn]
        public string Source { get; set; }

        [DataColumn]
        public bool Top { get; set; }

        [DataColumn]
        public bool Recommend { get; set; }

        [DataColumn]
        public bool Hot { get; set; }

        [DataColumn]
        public bool Color { get; set; }

        [DataColumn]
        public bool Knowledge { get; set; }

        [DataColumn]
        public bool Indexed { get; set; }

        [DataColumn]
        public LinkType LinkType { get; set; }

        [DataColumn]
        public string LinkUrl { get; set; }

        [DataColumn]
        public DateTime? AddDate { get; set; }
    }
}
