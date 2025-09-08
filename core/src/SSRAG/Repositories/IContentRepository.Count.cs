using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Datory;
using SqlKata;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Repositories
{
    public partial interface IContentRepository
    {
        Task<int> GetCountAsync(Site site, IChannelSummary channel);

        Task<int> GetCountOfCheckedAsync(Site site, IChannelSummary channel);

        Task<int> GetCountOfUnCheckedAsync(Site site, IChannelSummary channel);

        Task<int> GetCountCheckedImageAsync(Site site, Channel channel);

        Task<int> GetCountOfCheckedImagesAsync(Site site, IChannelSummary channel);

        Task<int> GetCountAsync(string tableName, Query query);

        Task<int> GetCountOfContentAddAsync(string tableName, int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, string adminName, bool? checkedState);

        Task<int> GetCountCheckingAsync(Site site);

        Task<int> GetCountCheckingAsync(Site site, List<int> channelIds);
    }
}