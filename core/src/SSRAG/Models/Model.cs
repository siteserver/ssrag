using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using System.Collections.Generic;

namespace SSRAG.Models
{
    [DataTable("ssrag_model")]
    public class Model : Entity
    {
        [DataColumn]
        public string ProviderId { get; set; }

        [DataColumn]
        public string ModelType { get; set; }

        [DataColumn]
        public string ModelId { get; set; }

        [DataColumn]
        public List<string> Skills { get; set; }
    }
}
