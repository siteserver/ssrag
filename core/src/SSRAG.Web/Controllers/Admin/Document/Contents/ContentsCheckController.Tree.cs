﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using System.Collections.Generic;
using SSRAG.Configuration;
using SSRAG.Utils;
using System.Linq;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsCheckController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<ActionResult<TreeResult>> Tree([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.ContentsCheck))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.SiteId);

            var enabledChannelIds = await _authManager.GetContentPermissionsChannelIdsAsync(site.Id);
            var visibleChannelIds = await _authManager.GetVisibleChannelIdsAsync(enabledChannelIds);

            var firstEnabledChannelId = 0;
            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var visible = visibleChannelIds.Contains(summary.Id);
                if (!visible) return null;

                var count = await _documentRepository.GetCountAsync(site.Id, summary.Id);
                // var count = await _contentRepository.GetCountOfUnCheckedAsync(site, summary);
                var disabled = !enabledChannelIds.Contains(summary.Id) && summary.Id != site.Id;
                if (firstEnabledChannelId == 0 && !disabled && summary.Id != site.Id)
                {
                    firstEnabledChannelId = summary.Id;
                }

                return new
                {
                    Count = count,
                    Disabled = disabled
                };
            });

            var siteUrl = _pathManager.GetSiteUrl(site, true);
            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);
            var allLevels = CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true);
            var levels = new List<KeyValuePair<int, string>>();
            foreach (var level in allLevels)
            {
                if (level.Key == CheckManager.LevelInt.CaoGao || level.Key == site.CheckContentLevel)
                {
                    continue;
                }
                levels.Add(level);
            }
            var checkedLevels = ElementUtils.GetCheckBoxes(levels);

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.CheckContents);

            var titleColumn =
                columns.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.AttributeName, nameof(Models.Content.Title)));
            columns.Remove(titleColumn);
            var bodyColumn = new ContentColumn
            {
                AttributeName = nameof(Models.Content.Body),
                DisplayName = "内容正文",
                InputType = InputType.TextEditor,
                IsSearchable = true,
            };

            var permissions = new Permissions
            {
                IsAdd = await _authManager.HasContentPermissionsAsync(site.Id, firstEnabledChannelId, MenuUtils.ContentPermissions.Add),
                IsDelete = await _authManager.HasContentPermissionsAsync(site.Id, firstEnabledChannelId, MenuUtils.ContentPermissions.Delete),
                IsEdit = await _authManager.HasContentPermissionsAsync(site.Id, firstEnabledChannelId, MenuUtils.ContentPermissions.Edit),
                IsArrange = await _authManager.HasContentPermissionsAsync(site.Id, firstEnabledChannelId, MenuUtils.ContentPermissions.Arrange),
                IsTranslate = await _authManager.HasContentPermissionsAsync(site.Id, firstEnabledChannelId, MenuUtils.ContentPermissions.Translate),
                IsCheck = await _authManager.HasContentPermissionsAsync(site.Id, firstEnabledChannelId, MenuUtils.ContentPermissions.CheckLevel1),
                IsCreate = await _authManager.HasSitePermissionsAsync(site.Id, MenuUtils.SitePermissions.CreateContents) || await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.Create),
            };

            return new TreeResult
            {
                Root = root,
                SiteUrl = siteUrl,
                GroupNames = groupNames,
                TagNames = tagNames,
                CheckedLevels = checkedLevels,
                Columns = columns,
                TitleColumn = titleColumn,
                BodyColumn = bodyColumn,
                Permissions = permissions
            };
        }
    }
}
