using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata;
using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IChannelRepository
    {
        Task<List<KeyValuePair<int, Channel>>> ParserGetChannelsAsync(int siteId, int pageChannelId, string group,
            string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, TaxisType order,
            ScopeType scopeType, bool isTotal, Query query);
    }
}
