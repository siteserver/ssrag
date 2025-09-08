using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Utils;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Datory;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesAddController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] SiteType siteType, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error(Constants.ErrorUpload);
            var extension = PathUtils.GetExtension(file.FileName);
            if (!FileUtils.IsZip(extension))
            {
                return this.Error("知识库模板压缩包为zip格式，请选择有效的文件上传!");
            }
            var directoryName = PathUtils.GetFileNameWithoutExtension(file.FileName);
            var directoryPath = _pathManager.GetSiteFilesPath(PathUtils.Combine(DirectoryUtils.SiteFiles.Templates.DirectoryName, siteType.GetValue(), directoryName));
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            var filePath = _pathManager.GetSiteFilesPath(PathUtils.Combine(DirectoryUtils.SiteFiles.Templates.DirectoryName, siteType.GetValue(), file.FileName));
            FileUtils.DeleteFileIfExists(filePath);

            await _pathManager.UploadAsync(file, filePath);
            _pathManager.ExtractZip(filePath, directoryPath);

            return new UploadResult
            {
                DirectoryName = directoryName
            };
        }
    }
}
