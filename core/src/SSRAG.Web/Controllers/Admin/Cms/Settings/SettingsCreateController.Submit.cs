﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsCreate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.IsCreateDoubleClick = request.IsCreateDoubleClick;
            site.IsCreateShowPageInfo = request.IsCreateShowPageInfo;
            site.IsCreateIe8Compatible = request.IsCreateIe8Compatible;
            site.IsCreateBrowserNoCache = request.IsCreateBrowserNoCache;
            site.IsCreateJsIgnoreError = request.IsCreateJsIgnoreError;
            site.IsCreateWithJQuery = request.IsCreateWithJQuery;
            site.IsCreateDisableFileDownloadApi = request.IsCreateDisableFileDownloadApi;
            site.IsCreateFilterGray = request.IsCreateFilterGray;
            site.CreateStaticMaxPage = request.CreateStaticMaxPage;
            site.IsCreateUseDefaultFileName = request.IsCreateUseDefaultFileName;
            site.CreateDefaultFileName = request.CreateDefaultFileName;
            site.IsCreateStaticContentByAddDate = request.IsCreateStaticContentByAddDate;
            site.CreateStaticContentAddDate = request.CreateStaticContentAddDate;

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改页面生成设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}