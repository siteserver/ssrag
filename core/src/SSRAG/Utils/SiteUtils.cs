using SSRAG.Enums;

namespace SSRAG.Utils
{
    public static class SiteUtils
    {
        public static bool IsContentTable(SiteType siteType)
        {
            return siteType == SiteType.Web;
        }
    }
}