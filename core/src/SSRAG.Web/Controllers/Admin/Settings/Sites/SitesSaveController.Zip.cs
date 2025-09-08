using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesSaveController
    {
        [HttpPost, Route(RouteZip)]
        public async Task<ActionResult<StringResult>> Zip([FromBody] ZipRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var directoryName = PathUtils.RemoveParentPath(request.DirectoryName);
            var fileName = directoryName + ".zip";
            var filePath = _pathManager.GetSiteTemplatesPath(request.SiteType, fileName);
            var directoryPath = _pathManager.GetSiteTemplatesPath(request.SiteType, directoryName);

            FileUtils.DeleteFileIfExists(filePath);

            _pathManager.CreateZip(filePath, directoryPath);

            return new StringResult
            {
                Value = _pathManager.GetSiteTemplatesUrl(request.SiteType, fileName)
            };
        }
    }
}
