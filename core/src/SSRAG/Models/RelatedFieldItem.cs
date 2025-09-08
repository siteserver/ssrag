using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_related_field_item")]
    public class RelatedFieldItem : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int RelatedFieldId { get; set; }

        [DataColumn]
        public string Label { get; set; }

        [DataColumn]
        public string Value { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }
    }
}
