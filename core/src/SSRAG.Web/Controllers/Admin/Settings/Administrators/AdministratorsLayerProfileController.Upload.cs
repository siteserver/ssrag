using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerProfileController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] int userId, [FromForm] IFormFile file)
        {
            var adminName = _authManager.AdminName;
            var admin = await _administratorRepository.GetByUserNameAsync(adminName);
            if (admin != null && admin.Id != userId &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error(Constants.ErrorUpload);
            var fileName = _pathManager.GetUploadFileName(file.FileName);

            var filePath = _pathManager.GetAdministratorUploadPath(adminName, fileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error(Constants.ErrorImageExtensionAllowed);
            }

            await _pathManager.UploadAsync(file, filePath);

            var avatarUrl = _pathManager.GetAdministratorUploadUrl(adminName, fileName);

            return new StringResult
            {
                Value = avatarUrl
            };
        }
    }
}
