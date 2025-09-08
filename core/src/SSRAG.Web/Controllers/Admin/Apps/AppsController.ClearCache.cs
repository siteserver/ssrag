using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Apps
{
    public partial class AppsController
    {
        [HttpPost, Route(RouteClearCache)]
        public async Task<ActionResult<BoolResult>> ClearCache([FromBody] SiteRequest request)
        {
            await _siteRepository.ClearCacheAsync(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
