using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesController
    {
        [HttpPost, Route(RouteUnZip)]
        public async Task<ActionResult<ListResult>> UnZip([FromBody] UnZipRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var fileNameToUnZip = PathUtils.RemoveParentPath(request.FileName);

            var directoryPathToUnZip = _pathManager.GetSiteTemplatesPath(request.SiteType, PathUtils.GetFileNameWithoutExtension(fileNameToUnZip));
            var zipFilePath = _pathManager.GetSiteTemplatesPath(request.SiteType, fileNameToUnZip);

            _pathManager.ExtractZip(zipFilePath, directoryPathToUnZip);

            return await GetListResultAsync(request.SiteType);
        }
    }
}
