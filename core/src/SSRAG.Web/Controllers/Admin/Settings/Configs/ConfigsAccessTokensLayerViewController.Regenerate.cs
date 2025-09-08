using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsAccessTokensLayerViewController
    {
        [HttpPost, Route(RouteRegenerate)]
        public async Task<ActionResult<RegenerateResult>> Regenerate([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsConfigsAccessTokens))
            {
                return Unauthorized();
            }

            var accessTokenInfo = await _accessTokenRepository.GetAsync(request.Id);

            var accessToken = _settingsManager.Decrypt(await _accessTokenRepository.RegenerateAsync(accessTokenInfo));

            return new RegenerateResult
            {
                AccessToken = accessToken
            };
        }
    }
}
