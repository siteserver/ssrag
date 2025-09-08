using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Core.Utils.Serialization;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<List<int>>> Import([FromBody] ImportRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            try
            {
                var site = await _siteRepository.GetAsync(request.SiteId);

                var fileName = PathUtils.RemoveParentPath(request.FileName);
                var filePath = _pathManager.GetTemporaryFilesPath(fileName);
                var adminName = _authManager.AdminName;
                var caching = new Caching(_settingsManager);

                var importObject = new ImportObject(_pathManager, _databaseManager, caching, site, adminName);
                await importObject.ImportChannelsAndContentsByZipFileAsync(request.ChannelId, filePath,
                    request.IsOverride, request.IsContents, null);

                await _authManager.AddSiteLogAsync(request.SiteId, "导入栏目");
            }
            catch
            {
                return this.Error("压缩包格式不正确，请上传正确的栏目压缩包");
            }

            return new List<int>
            {
                request.SiteId,
                request.ChannelId
            };
        }
    }
}
