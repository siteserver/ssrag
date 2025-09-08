using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Editor
{
    public partial class ImageEditorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> Submit([FromBody] SubmitRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            byte[] data = Convert.FromBase64String(request.Base64String);
            using var image = Image.Load(data);
            // image.SaveAsPngAsync()

            var fileName = "image.png";
            var extName = PathUtils.GetExtension(fileName);

            var localDirectoryPath = _pathManager.GetUploadDirectoryPath(site, UploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, fileName));

            await image.SaveAsPngAsync(filePath);
            var url = _pathManager.GetVirtualUrlByPhysicalPath(site, filePath);

            return new StringResult
            {
                Value = url,
            };
        }
    }
}
