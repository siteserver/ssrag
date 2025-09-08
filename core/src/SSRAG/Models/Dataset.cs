using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_dataset")]
    public class Dataset : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string NodeId { get; set; }

        [DataColumn]
        public int DatasetSiteId { get; set; }

        [DataColumn]
        public bool DatasetAllChannels { get; set; }

        [DataColumn]
        public List<int> DatasetChannelIds { get; set; }
    }
}