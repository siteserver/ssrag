using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Core.Utils.Serialization;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] ExportRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var caching = new Caching(_settingsManager);
            var exportObject = new ExportObject(_pathManager, _databaseManager, caching, site);
            var fileName = await exportObject.ExportChannelsAsync(request.ChannelIds, request.IsContents, request.IsFileImages, request.IsFileAttaches);
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var url = _pathManager.GetDownloadApiUrl(filePath);

            return new StringResult
            {
                Value = url
            };
        }
    }
}
