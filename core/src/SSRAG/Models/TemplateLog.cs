using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_template_log")]
    public class TemplateLog : Entity
    {
        [DataColumn]
        public int TemplateId { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string AdminName { get; set; }

        [DataColumn]
        public int ContentLength { get; set; }

        [DataColumn(Text = true)]
        public string TemplateContent { get; set; }
    }
}
