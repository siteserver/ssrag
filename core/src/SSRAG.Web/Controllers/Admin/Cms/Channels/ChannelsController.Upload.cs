﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] int siteId, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = PathUtils.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return this.Error("导入文件为Zip格式，请选择有效的文件上传");
            }

            fileName = $"{StringUtils.GetShortUuid(false)}.zip";
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);

            await _pathManager.UploadAsync(file, filePath);

            return new StringResult
            {
                Value = fileName
            };
        }
    }
}
