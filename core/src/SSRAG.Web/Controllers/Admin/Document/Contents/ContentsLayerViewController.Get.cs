using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            var content = await _pathManager.DecodeContentAsync(site, channel, request.ContentId);
            if (content == null) return this.Error(Constants.ErrorNotFound);

            var channelName = await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId);

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);

            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            var calculatedContent =
                await columnsManager.CalculateContentListAsync(1, site, request.ChannelId, content, columns);
            calculatedContent.Body = content.Body;

            var siteUrl = _pathManager.GetSiteUrl(site, true);
            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);

            var editorColumns = new List<ContentColumn>();

            var styles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            foreach (var tableStyle in styles)
            {
                if (tableStyle.InputType != InputType.TextEditor || tableStyle.AttributeName == nameof(Models.Content.Body)) continue;

                editorColumns.Add(new ContentColumn
                {
                    AttributeName = tableStyle.AttributeName,
                    DisplayName = tableStyle.DisplayName
                });
            }

            var isCheckable = await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ContentsCheck);

            return new GetResult
            {
                Content = calculatedContent,
                ChannelName = channelName,
                State = CheckManager.GetCheckState(site, content),
                Columns = columns,
                SiteUrl = siteUrl,
                GroupNames = groupNames,
                TagNames = tagNames,
                EditorColumns = editorColumns,
                IsCheckable = isCheckable,
            };
        }
    }
}