﻿using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IContentRepository
    {
        Task UpdateHitsAsync(int siteId, int channelId, int contentId, int hits);

        Task UpdateAsync(Content content);

        Task UpdateAsync(Site site, Channel channel, Content content);

        Task SetAutoPageContentToSiteAsync(Site site);

        Task UpdateArrangeTaxisAsync(Site site, Channel channel, string attributeName, bool isDesc);

        Task SetTaxisAsync(Site site, Channel channel, int contentId, bool isTop, int order);

        Task<bool> SetTaxisToUpAsync(Site site, Channel channel, int contentId, bool isTop);

        Task<bool> SetTaxisToDownAsync(Site site, Channel channel, int contentId, bool isTop);

        Task AddDownloadsAsync(string tableName, int channelId, int contentId);
    }
}
