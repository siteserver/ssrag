using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_celery_task")]
    public class CeleryTask : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public string TaskId { get; set; }

        [DataColumn]
        public string TaskStatus { get; set; }

        [DataColumn]
        public string TaskResult { get; set; }
    }
}
