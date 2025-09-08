using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsAccessTokensController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<TokensResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsConfigsAccessTokens))
            {
                return Unauthorized();
            }

            await _accessTokenRepository.DeleteAsync(request.Id);
            var list = await _accessTokenRepository.GetAccessTokensAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }
    }
}
