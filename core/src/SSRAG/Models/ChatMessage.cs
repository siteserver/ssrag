using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_chat_message")]
    public class ChatMessage : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string SessionId { get; set; }

        [DataColumn]
        public string Role { get; set; }

        [DataColumn(Text = true)]
        public string Reasoning { get; set; }

        [DataColumn(Text = true)]
        public string Content { get; set; }
    }
}
