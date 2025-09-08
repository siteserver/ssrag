﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Models;
using SSRAG.Core.Utils;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpPost, Route(RouteItems)]
        public async Task<ActionResult<ItemsResult>> ItemsAdd([FromBody] ItemsAddRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            if (request.IsRapid)
            {
                foreach (var rapidValue in ListUtils.GetStringListByReturnAndNewline(request.RapidValues))
                {
                    if (string.IsNullOrWhiteSpace(rapidValue)) continue;

                    var itemInfo = new RelatedFieldItem
                    {
                        Id = 0,
                        SiteId = request.SiteId,
                        RelatedFieldId = request.RelatedFieldId,
                        Label = rapidValue,
                        Value = rapidValue,
                        ParentId = request.ParentId
                    };
                    await _relatedFieldItemRepository.InsertAsync(itemInfo);
                }
            }
            else
            {
                foreach (var item in request.Items)
                {
                    var itemInfo = new RelatedFieldItem
                    {
                        Id = 0,
                        SiteId = request.SiteId,
                        RelatedFieldId = request.RelatedFieldId,
                        Label = item.Key,
                        Value = item.Value,
                        ParentId = request.ParentId
                    };
                    await _relatedFieldItemRepository.InsertAsync(itemInfo);
                }
            }

            await _authManager.AddAdminLogAsync("批量添加联动字段项");

            var tree = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, request.RelatedFieldId,
                0);

            return new ItemsResult
            {
                Tree = tree
            };
        }
    }
}
