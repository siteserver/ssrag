using System.Threading.Tasks;
using SSRAG.Datory;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Datory.Utils;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteRedisConnect)]
        public async Task<ActionResult<BoolResult>> RedisConnect([FromBody] RedisConnectRequest request)
        {
            if (!await _configRepository.IsNeedInstallAsync()) return Unauthorized();

            var redisConnectionString = string.Empty;
            if (_settingsManager.Containerized)
            {
                redisConnectionString = _settingsManager.RedisConnectionString;
            }
            else
            {
                if (request.IsRedis)
                {
                    redisConnectionString = Utilities.GetRedisConnectionString(request.RedisHost, request.IsRedisDefaultPort, request.RedisPort, request.RedisPassword, request.IsSsl, request.RedisDatabase, string.Empty);
                }
            }

            var db = new DistributedCache(redisConnectionString);

            var (isConnectionWorks, message) = await db.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                return this.Error(message);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
