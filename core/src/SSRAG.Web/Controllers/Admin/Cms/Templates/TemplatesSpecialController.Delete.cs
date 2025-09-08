﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] SpecialIdRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var specialInfo = await _pathManager.DeleteSpecialAsync(site, request.SpecialId);

            await _authManager.AddSiteLogAsync(request.SiteId,
                "删除专题",
                $"专题名称：{specialInfo.Title}");

            var specials = await _specialRepository.GetSpecialsAsync(request.SiteId);
            foreach (var special in specials)
            {
                var filePath = PathUtils.Combine(_pathManager.GetSpecialDirectoryPath(site, special.Url), "index.html");
                special.Set("editable", FileUtils.IsFileExists(filePath));
            }

            return new DeleteResult
            {
                Specials = specials
            };
        }
    }
}