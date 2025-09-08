using SSRAG.Enums;

namespace SSRAG.Dto
{
    public class TemplateSummary
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public TemplateType TemplateType { get; set; }
        public bool DefaultTemplate{ get; set; }
    }
}
