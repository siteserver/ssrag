using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers
{
    public partial class PingController
    {
        [HttpGet, Route(RouteIp)]
        public async Task<ActionResult<string>> Ip()
        {
            return await RestUtils.GetIpAddressAsync();
        }
    }
}