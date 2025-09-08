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
        [HttpGet, Route(Constants.RoutePreviewContent)]
        public async Task<ActionResult> GetContent([FromRoute] int siteId, [FromRoute] int channelId, [FromRoute] int contentId, [FromQuery] GetContentRequest request)
        {
            try
            {
                var visualInfo = await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, siteId, channelId, contentId, 0, request.PageIndex, request.IsPreview);
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
