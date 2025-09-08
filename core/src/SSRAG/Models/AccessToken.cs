using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_access_token")]
    public class AccessToken : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Token { get; set; }

        [DataColumn]
        public string AdminName { get; set; }

        [DataColumn]
        public List<string> Scopes { get; set; }
    }
}
