﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTablesController
    {
        [HttpPost, Route(RouteTableActionsRemoveCache)]
        public async Task<ActionResult<GetColumnsResult>> RemoveCache(string tableName)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTables))
            {
                return Unauthorized();
            }

            tableName = StringUtils.FilterSqlAndXss(tableName);
            var columns = await _databaseManager.GetTableColumnInfoListAsync(tableName, ColumnsManager.MetadataAttributes.Value);

            return new GetColumnsResult
            {
                Columns = columns,
                Count = _databaseManager.GetCount(tableName)
            };
        }
    }
}
