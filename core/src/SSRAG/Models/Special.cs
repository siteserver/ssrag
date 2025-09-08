using System;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_special")]
    public class Special : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Url { get; set; }

        [DataColumn]
        public DateTime AddDate { get; set; }
    }
}
