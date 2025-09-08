using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_storage_file")]
    public class StorageFile : Entity
    {
        [DataColumn]
        public string Key { get; set; }

        [DataColumn]
        public string ETag { get; set; }

        [DataColumn]
        public string Md5 { get; set; }

        [DataColumn]
        public FileType FileType { get; set; }
    }
}
