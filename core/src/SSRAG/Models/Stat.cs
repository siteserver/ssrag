using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_stat")]
    public class Stat : Entity
    {
        [DataColumn]
        public StatType StatType { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string AdminName { get; set; }

        [DataColumn]
        public int Count { get; set; }
    }
}
