using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Form
{
    public partial class LayerImageUploadController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] int siteId, [FromQuery] string userName, [FromForm] IFormFile file)
        {
            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = PathUtils.GetFileName(file.FileName);
            var filePath = string.Empty;

            if (siteId > 0)
            {
                var site = await _siteRepository.GetAsync(siteId);

                (var success, filePath, var errorMessage) = await _pathManager.UploadImageAsync(site, file);
                if (!success)
                {
                    return this.Error(errorMessage);
                }
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                filePath = _pathManager.GetUserUploadPath(userName, fileName);
                if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
                {
                    return this.Error(Constants.ErrorImageExtensionAllowed);
                }

                await _pathManager.UploadAsync(file, filePath);
            }

            var rootUrl = _pathManager.GetRootUrlByPath(filePath);

            return new UploadResult
            {
                Name = fileName,
                Path = rootUrl
            };
        }
    }
}