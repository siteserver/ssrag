using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_segment")]
    public class Segment : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public int DocumentId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn(Text = true)]
        public string Text { get; set; }

        [DataColumn]
        public string TextHash { get; set; }
    }
}