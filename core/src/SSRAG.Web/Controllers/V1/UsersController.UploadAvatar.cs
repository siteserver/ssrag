using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("上传用户头像 API", "上传用户头像，使用POST发起请求，请求地址为/api/v1/users/{id}/avatar")]
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUserAvatar)]
        public async Task<ActionResult<User>> UploadAvatar([FromRoute] int id, [FromForm] IFormFile file)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return this.Error(Constants.ErrorNotFound);

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = PathUtils.GetFileName(file.FileName);

            fileName = _pathManager.GetUserUploadFileName(fileName);
            var filePath = _pathManager.GetUserUploadPath(user.UserName, fileName);

            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error(Constants.ErrorImageExtensionAllowed);
            }

            await _pathManager.UploadAsync(file, filePath);

            user.AvatarUrl = _pathManager.GetUserUploadUrl(user.UserName, fileName);

            await _userRepository.UpdateAsync(user);

            return user;
        }
    }
}
