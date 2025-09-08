using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using SSRAG.Utils;
using System.Linq;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsRecycleController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<ActionResult<TreeResult>> Tree([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.ContentsRecycle))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.SiteId);

            var channelId = 0;

            var channelIdList = await _authManager.GetContentPermissionsChannelIdsAsync(request.SiteId, MenuUtils.ContentPermissions.Add);
            var channels = await _channelRepository.GetAsync(request.SiteId);
            var root = await _channelRepository.GetCascadeAsync(site, channels, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var disabled = !channelIdList.Contains(summary.Id);
                if (channelId == 0 && !disabled)
                {
                    channelId = summary.Id;
                }

                return new
                {
                    Disabled = disabled,
                    Count = count
                };
            });

            var siteUrl = _pathManager.GetSiteUrl(site, true);
            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);
            var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.RecycleContents);

            var titleColumn =
                columns.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.AttributeName, nameof(Models.Content.Title)));
            var bodyColumn = new ContentColumn
            {
                AttributeName = nameof(Models.Content.Body),
                DisplayName = "内容正文",
                InputType = InputType.TextEditor,
                IsSearchable = true,
            };

            return new TreeResult
            {
                Root = root,
                ChannelId = channelId,
                SiteUrl = siteUrl,
                GroupNames = groupNames,
                TagNames = tagNames,
                CheckedLevels = checkedLevels,
                Columns = columns,
                TitleColumn = titleColumn,
                BodyColumn = bodyColumn,
            };
        }
    }
}
