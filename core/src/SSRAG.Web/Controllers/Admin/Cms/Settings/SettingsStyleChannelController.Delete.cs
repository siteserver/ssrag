﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleChannelController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleChannel))
            {
                return Unauthorized();
            }

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            await _tableStyleRepository.DeleteAsync(_channelRepository.TableName, request.ChannelId, request.AttributeName);

            var tableName = _channelRepository.TableName;
            var relatedIdentities = _tableStyleRepository.GetRelatedIdentities(channel);
            var styles = await _tableStyleRepository.GetTableStylesAsync(tableName, relatedIdentities);
            foreach (var style in styles)
            {
                style.IsSystem = style.RelatedIdentity != request.ChannelId;
            }

            return new DeleteResult
            {
                Styles = styles
            };
        }
    }
}
