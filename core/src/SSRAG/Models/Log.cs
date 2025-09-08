using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_log")]
    public class Log : Entity
    {
        [DataColumn]
        public int AdminId { get; set; }

        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public string IpAddress { get; set; }

        [DataColumn]
        public string Action { get; set; }

        [DataColumn]
        public string Summary { get; set; }
    }
}
