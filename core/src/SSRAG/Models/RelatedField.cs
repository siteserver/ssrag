using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_related_field")]
    public class RelatedField : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string Title { get; set; }
    }
}
