﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Configuration;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Document.Contents
{
    public partial class ContentsLayerImportController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return this.Error("无法确定内容对应的栏目");

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            var options = GetOptions(site);

            return new GetResult
            {
                Value = checkedLevel,
                CheckedLevels = checkedLevels,
                Options = options
            };
        }
    }
}