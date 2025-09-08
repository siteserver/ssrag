﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Core.Utils;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentGroupController
    {
        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<GetResult>> Edit([FromBody] ChannelGroup request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsContentGroup))
            {
                return Unauthorized();
            }

            var groupInfo = await _contentGroupRepository.GetAsync(request.SiteId, request.Id);

            if (groupInfo.GroupName != request.GroupName && await _contentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的内容组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await _contentGroupRepository.UpdateAsync(groupInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组：{groupInfo.GroupName}");

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}