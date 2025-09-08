using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_model_provider")]
    public class ModelProvider : Entity
    {
        [DataColumn]
        public string ProviderId { get; set; }

        [DataColumn]
        public string ProviderName { get; set; }

        [DataColumn]
        public string IconUrl { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [JsonIgnore]
        [DataColumn(Text = true)]
        public string Credentials { get; set; }

        [JsonIgnore]
        [DataColumn]
        public string CredentialsSalt { get; set; }
    }
}
