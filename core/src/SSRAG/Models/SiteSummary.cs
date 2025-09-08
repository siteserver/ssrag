using SSRAG.Enums;

namespace SSRAG.Models
{
    public class SiteSummary
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        public SiteType SiteType { get; set; }
        public string IconUrl { get; set; }
        public string SiteDir { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; }
        public bool Root { get; set; }
        public bool Disabled { get; set; }
        public int Taxis { get; set; }
    }
}
