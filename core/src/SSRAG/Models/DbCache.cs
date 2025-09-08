using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_db_cache")]
    public class DbCache : Entity
    {
        [DataColumn]
        public string CacheKey { get; set; }

        [DataColumn]
        public string CacheValue { get; set; }
    }
}
