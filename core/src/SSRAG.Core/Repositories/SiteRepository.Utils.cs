using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Enums;

namespace SSRAG.Core.Repositories
{
    public partial class SiteRepository
    {
        public async Task<List<int>> GetLatestSiteIdsAsync(List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions)
        {
            var siteIdList = new List<int>();

            foreach (var siteId in siteIdListLatestAccessed)
            {
                if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                {
                    siteIdList.Add(siteId);
                }
            }

            var siteIds = await GetSiteIdsAsync();
            foreach (var siteId in siteIds)
            {
                if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                {
                    siteIdList.Add(siteId);
                }
            }

            return siteIdList;
        }

        public string GetSiteTypeIconClass(SiteType siteType)
        {
            switch (siteType)
            {
                case SiteType.Web:
                    return "ion-earth";
                case SiteType.Markdown:
                    return "ion-document";
                case SiteType.Document:
                    return "el-icon-document-copy";
                case SiteType.Chat:
                    return "el-icon-chat-line-round";
                case SiteType.Chatflow:
                    return "el-icon-guide";
                case SiteType.Agent:
                    return "ion-cube";
                default:
                    return "ion-earth";
            }

            return "ion-earth";
        }
    }
}
