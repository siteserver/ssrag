using System;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_form_data")]
    public class FormData : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public int FormId { get; set; }

        [DataColumn]
        public bool IsReplied { get; set; }

        [DataColumn]
        public DateTime? ReplyDate { get; set; }

        [DataColumn(Text = true)]
        public string ReplyContent { get; set; }
    }
}