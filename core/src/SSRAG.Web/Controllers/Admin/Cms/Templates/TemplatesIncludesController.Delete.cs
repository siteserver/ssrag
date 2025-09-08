using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;
using SSRAG.Core.Utils;
using SSRAG.Configuration;

namespace SSRAG.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesIncludesController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] FileRequest request)
        {
            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafeMode);
            }

            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesIncludes))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var directoryPath = PathUtils.RemoveParentPath(request.DirectoryPath);
            var fileName = PathUtils.RemoveParentPath(request.FileName);

            FileUtils.DeleteFileIfExists(_pathManager.GetSitePath(site, directoryPath, fileName));
            await _authManager.AddSiteLogAsync(request.SiteId, "删除包含文件", $"{directoryPath}:{fileName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
