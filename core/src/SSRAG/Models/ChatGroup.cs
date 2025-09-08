using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_chat_group")]
    public class ChatGroup : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string SessionId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string AdminName { get; set; }

        [DataColumn]
        public string UserName { get; set; }

        [DataColumn]
        public bool IsDeleted { get; set; }
    }
}
