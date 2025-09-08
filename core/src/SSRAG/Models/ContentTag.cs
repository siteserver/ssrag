using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_content_tag")]
    public class ContentTag : Entity
    {
        [DataColumn]
        public string TagName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn(Text = true)]
        public List<int> ContentIds { get; set; }

        [DataColumn]
        public int UseNum { get; set; }

        public int Level { get; set; }
    }
}
