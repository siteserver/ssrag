using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_error_log")]
    public class ErrorLog : Entity
    {
        [DataColumn]
        public string Category { get; set; }

        [DataColumn]
        public string PluginId { get; set; }

        [DataColumn]
        public string Message { get; set; }

        [DataColumn(Text = true)]
        public string StackTrace { get; set; }

        [DataColumn(Text = true)]
        public string Summary { get; set; }
    }
}
