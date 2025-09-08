using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsAccessTokensLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int id)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsConfigsAccessTokens))
            {
                return Unauthorized();
            }

            var tokenInfo = await _accessTokenRepository.GetAsync(id);
            var accessToken = _settingsManager.Decrypt(tokenInfo.Token);

            return new GetResult
            {
                Token = tokenInfo,
                AccessToken = accessToken
            };
        }
    }
}
