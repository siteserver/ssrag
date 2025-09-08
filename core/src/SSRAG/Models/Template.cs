using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_template")]
    public class Template : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string TemplateName { get; set; }

        [DataColumn]
        public TemplateType TemplateType { get; set; }

        [DataColumn]
        public string RelatedFileName { get; set; }

        [DataColumn]
        public string CreatedFileFullName { get; set; }

        [DataColumn]
        public string CreatedFileExtName { get; set; }

        [DataColumn]
        public bool DefaultTemplate { get; set; }

        [DataIgnore]
        public string Content { get; set; }
    }
}