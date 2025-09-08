using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_document")]
    public class Document : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string DirPath { get; set; }

        [DataColumn]
        public string FileName { get; set; }

        [DataColumn]
        public string ExtName { get; set; }

        [DataColumn]
        public int FileSize { get; set; }

        [DataColumn]
        public string Separators { get; set; }

        [DataColumn]
        public int ChunkSize { get; set; }

        [DataColumn]
        public int ChunkOverlap { get; set; }

        [DataColumn]
        public bool IsChunkReplaces { get; set; }

        [DataColumn]
        public bool IsChunkDeletes { get; set; }
    }
}