using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_translate")]
    public class Translate : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int TargetSiteId { get; set; }

        [DataColumn]
        public int TargetChannelId { get; set; }

        [DataColumn]
        public TranslateType TranslateType { get; set; }

        [DataIgnore]
        public string Summary { get; set; }
    }
}