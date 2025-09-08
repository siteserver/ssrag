using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteSaveIds)]
        public async Task<ActionResult<StringResult>> SaveIds([FromBody] SaveIdsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Contents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var fileName = await _pathManager.WriteTemporaryTextAsync(TranslateUtils.JsonSerialize(request.ChannelContentIds));

            return new StringResult
            {
                Value = fileName
            };
        }
    }
}
