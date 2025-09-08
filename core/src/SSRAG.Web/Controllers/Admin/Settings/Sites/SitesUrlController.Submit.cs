using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesUrlController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesUrl))
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(request.SeparatedWebUrl) && !request.SeparatedWebUrl.EndsWith("/"))
            {
                request.SeparatedWebUrl = request.SeparatedWebUrl + "/";
            }

            if (!string.IsNullOrEmpty(request.SeparatedApiUrl) && !request.SeparatedApiUrl.EndsWith("/"))
            {
                request.SeparatedApiUrl = request.SeparatedApiUrl + "/";
            }
            if (StringUtils.EndsWithIgnoreCase(request.SeparatedApiUrl, "/api/"))
            {
                request.SeparatedApiUrl =
                    StringUtils.ReplaceEndsWithIgnoreCase(request.SeparatedApiUrl, "/api/", string.Empty);
            }
            if (!string.IsNullOrEmpty(request.SeparatedApiUrl) && !request.SeparatedApiUrl.EndsWith("/"))
            {
                request.SeparatedApiUrl = request.SeparatedApiUrl + "/";
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.IsSeparatedWeb = request.IsSeparatedWeb;
            site.SeparatedWebUrl = request.SeparatedWebUrl;

            site.IsSeparatedAssets = request.IsSeparatedAssets;
            site.SeparatedAssetsUrl = request.SeparatedAssetsUrl;
            site.AssetsDir = request.AssetsDir;

            site.IsSeparatedApi = request.IsSeparatedApi;
            site.SeparatedApiUrl = request.SeparatedApiUrl;

            await _siteRepository.UpdateAsync(site);
            await _authManager.AddSiteLogAsync(request.SiteId, "修改知识库访问地址");

            return new BoolResult { Value = true };
        }
    }
}
