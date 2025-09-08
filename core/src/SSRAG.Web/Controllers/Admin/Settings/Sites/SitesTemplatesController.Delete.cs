using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var caching = new Caching(_settingsManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching, request.SiteType);

            var directoryName = PathUtils.RemoveParentPath(request.DirectoryName);
            var fileName = PathUtils.RemoveParentPath(request.FileName);
            if (!string.IsNullOrEmpty(directoryName))
            {
                manager.DeleteSiteTemplate(directoryName);
                await _authManager.AddAdminLogAsync("删除知识库模板", $"知识库模板:{directoryName}");
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                manager.DeleteZipSiteTemplate(fileName);
                await _authManager.AddAdminLogAsync("删除未解压知识库模板", $"知识库模板:{fileName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
