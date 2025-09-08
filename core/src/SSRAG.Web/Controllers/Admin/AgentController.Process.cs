using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteProcess)]
        public async Task<ActionResult<Process>> Process([FromBody] ProcessRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

            var caching = new Caching(_settingsManager);
            return await caching.GetProcessAsync(request.Uuid);
        }
    }
}