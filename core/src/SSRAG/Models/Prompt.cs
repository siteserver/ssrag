using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_prompt")]
    public class Prompt : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public PromptPosition Position { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string IconUrl { get; set; }

        [DataColumn]
        public string Text { get; set; }

        [DataColumn]
        public int Taxis { get; set; }
    }
}
