using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Home
{
    public partial class HomeConfigController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromForm] IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsHomeConfig))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error(Constants.ErrorUpload);
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error(Constants.ErrorImageExtensionAllowed);
            }
            var filePath = _pathManager.GetHomeUploadPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var url = _pathManager.GetHomeUploadUrl(fileName);

            return new StringResult
            {
                Value = url
            };
        }
    }
}
