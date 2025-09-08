﻿using System;
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
        [HttpGet, Route(Constants.RoutePreviewFile)]
        public async Task<ActionResult> GetFile([FromRoute] int siteId, [FromRoute] int fileTemplateId, [FromQuery] GetFileRequest request)
        {
            try
            {
                var visualInfo = await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, siteId, 0, 0, fileTemplateId, request.PageIndex);
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
