﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsSiteController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsSite))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var entity = new Entity();
            var styles = await GetInputStylesAsync(request.SiteId);
            var relatedFields = new Dictionary<int, List<Cascade<int>>>();

            foreach (var style in styles)
            {
                if (style.InputType == InputType.Image ||
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    var count = site.Get(ColumnsManager.GetCountName(style.AttributeName), 0);
                    entity.Set(ColumnsManager.GetCountName(style.AttributeName), count);
                    for (var n = 0; n <= count; n++)
                    {
                        var extendName = ColumnsManager.GetExtendName(style.AttributeName, n);
                        entity.Set(extendName, site.Get(extendName));
                    }
                }
                else if (style.InputType == InputType.CheckBox ||
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = ListUtils.GetStringList(site.Get(style.AttributeName,
                        string.Empty));
                    entity.Set(style.AttributeName, list);
                }
                else if (style.InputType == InputType.SelectCascading)
                {
                    if (style.RelatedFieldId > 0)
                    {
                        var items = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, style.RelatedFieldId, 0);
                        relatedFields[style.RelatedFieldId] = items;
                    }
                    entity.Set(style.AttributeName, site.Get(style.AttributeName));
                }
                else if (style.InputType == InputType.Date || style.InputType == InputType.DateTime)
                {
                    var date = TranslateUtils.ToDateTime(site.Get<string>(style.AttributeName), DateTime.Now);
                    entity.Set(style.AttributeName, date);
                }
                else
                {
                    entity.Set(style.AttributeName, site.Get(style.AttributeName));
                }
            }

            var siteUrl = _pathManager.GetSiteUrl(site, true);

            return new GetResult
            {
                SiteUrl = StringUtils.TrimEndSlash(siteUrl),
                Entity = entity,
                Styles = styles,
                RelatedFields = relatedFields,
            };
        }
    }
}