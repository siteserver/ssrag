using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class IndexController
    {
        [Authorize(Roles = Types.Roles.Administrator)]
        [HttpPost, Route(RouteCache)]
        public async Task<ActionResult<IntResult>> Cache([FromBody] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            await _channelRepository.CacheAllAsync(site);
            var channelSummaries = await _channelRepository.GetSummariesAsync(site.Id);
            await _contentRepository.CacheAllListAndCountAsync(site, channelSummaries);
            await _contentRepository.CacheAllEntityAsync(site, channelSummaries);

            return new IntResult
            {
                Value = channelSummaries.Count
            };
        }
    }
}
