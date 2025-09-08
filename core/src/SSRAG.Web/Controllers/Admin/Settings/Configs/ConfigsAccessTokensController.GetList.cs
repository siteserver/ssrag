using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Models;

namespace SSRAG.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsAccessTokensController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> GetList()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsConfigsAccessTokens))
            {
                return Unauthorized();
            }

            var adminName = _authManager.AdminName;
            var adminNames = new List<string>();

            if (await _authManager.IsSuperAdminAsync())
            {
                adminNames = await _administratorRepository.GetUserNamesAsync();
            }
            else
            {
                adminNames.Add(adminName);
            }

            var scopes = new List<string>
            {
                Constants.ScopeChannels,
                Constants.ScopeContents,
                Constants.ScopeSTL,
                Constants.ScopeAdministrators,
                Constants.ScopeUsers,
                Constants.ScopeOthers,
            };

            var accessTokens = await _accessTokenRepository.GetAccessTokensAsync();
            var tokens = new List<AccessToken>();
            foreach (var token in accessTokens)
            {
                var admin = await _administratorRepository.GetByUserNameAsync(token.AdminName);
                if (admin != null)
                {
                    token.Set("AdminDisplay", _administratorRepository.GetDisplay(admin));
                    token.Set("AdminUuid", admin.Uuid);
                    tokens.Add(token);
                }
            }

            return new ListResult
            {
                Tokens = tokens,
                AdminNames = adminNames,
                Scopes = scopes,
                AdminName = adminName
            };
        }
    }
}
