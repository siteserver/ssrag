using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Web.Controllers.Admin;

namespace SSRAG.Web.Controllers.Preview
{
    public partial class PreviewController
    {
        [HttpGet, Route(Constants.RoutePreviewChannel)]
        public async Task<ActionResult> GetChannel([FromRoute] int siteId, [FromRoute] int channelId, [FromQuery] GetChannelRequest request)
        {
            try
            {
                var visualInfo = await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, siteId, channelId, 0, 0, request.PageIndex);
                return await GetResponseMessageAsync(visualInfo);
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }
    }
}
