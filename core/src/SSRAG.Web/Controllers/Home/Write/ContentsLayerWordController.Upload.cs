﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Home.Write
{
    public partial class ContentsLayerWordController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<NameTitle>> Upload([FromQuery] ChannelRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var title = PathUtils.GetFileNameWithoutExtension(file.FileName);
            var fileName = PathUtils.GetUploadFileName(file.FileName, true);
            var extendName = PathUtils.GetExtension(fileName);

            if (!FileUtils.IsWord(extendName))
            {
                return this.Error("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            return new NameTitle
            {
                FileName = fileName,
                Title = title
            };
        }
    }
}
