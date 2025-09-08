using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Services;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Configuration;

namespace SSRAG.Web.Controllers.Admin.Common
{
    public partial class AdminLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            Administrator admin = null;
            if (!string.IsNullOrEmpty(request.Uuid))
            {
                admin = await _administratorRepository.GetByUuidAsync(request.Uuid);
            }

            if (admin == null) return this.Error(Constants.ErrorNotFound);

            admin.Remove("confirmPassword");

            var permissions = new AuthManager(_context, _antiforgery, _settingsManager, _databaseManager);
            await permissions.InitAsync(admin);
            var level = await permissions.GetAdminLevelAsync();
            var siteNames = new List<string>();
            var siteIdListWithPermissions = await permissions.GetSiteIdsAsync();
            foreach (var siteId in siteIdListWithPermissions)
            {
                var site = await _siteRepository.GetAsync(siteId);
                if (site != null)
                {
                    siteNames.Add(site.SiteName);
                }
            }
            var roleNames = await _administratorRepository.GetRolesAsync(admin.UserName);

            return new GetResult
            {
                Administrator = admin,
                Level = level,
                SiteNames = ListUtils.ToString(siteNames, "<br />"),
                RoleNames = roleNames
            };
        }
    }
}
