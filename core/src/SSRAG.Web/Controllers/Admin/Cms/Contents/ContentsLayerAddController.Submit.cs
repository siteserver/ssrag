﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Utils;
using SSRAG.Core.Utils;
using System.Collections.Specialized;
using SSRAG.Configuration;

namespace SSRAG.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;

            var adminName = _authManager.AdminName;
            var contentIdList = new List<int>();

            var titles = ListUtils.GetStringListByReturnAndNewline(request.Titles);
            titles.Reverse();

            foreach (var value in titles)
            {
                if (string.IsNullOrWhiteSpace(value)) continue;

                var title = string.Empty;
                var body = string.Empty;
                var attributes = new NameValueCollection();

                if (value.Contains('(') && value.Contains(')'))
                {
                    var length = value.IndexOf(')') - value.IndexOf('(') - 1;
                    if (length > 0)
                    {
                        var separateString = value.Substring(value.IndexOf('(') + 1, length);
                        if (StringUtils.Contains(separateString, "="))
                        {
                            attributes = TranslateUtils.ToNameValueCollection(separateString);
                        }
                        else
                        {
                            body = separateString;
                        }
                        title = value.Substring(0, value.IndexOf('('));
                    }
                }
                else if (value.Contains('（') && value.Contains('）'))
                {
                    var length = value.IndexOf('）') - value.IndexOf('（') - 1;
                    if (length > 0)
                    {
                        var separateString = value.Substring(value.IndexOf('（') + 1, length);
                        if (StringUtils.Contains(separateString, "="))
                        {
                            attributes = TranslateUtils.ToNameValueCollection(separateString);
                        }
                        else
                        {
                            body = separateString;
                        }
                        title = value.Substring(0, value.IndexOf('（'));
                    }
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = value.Trim();
                }

                var content = new Content
                {
                    ChannelId = channel.Id,
                    SiteId = request.SiteId,
                    AdminName = adminName,
                    LastEditAdminName = adminName,
                    AddDate = DateTime.Now,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel,
                    Title = StringUtils.Trim(title),
                    ImageUrl = string.Empty,
                    Body = body
                };
                foreach (string key in attributes.Keys)
                {
                    content.Set(key, attributes[key]);
                }

                await _contentRepository.InsertAsync(site, channel, content);
                contentIdList.Add(content.Id);
            }

            if (isChecked)
            {
                foreach (var contentId in contentIdList)
                {
                    await _createManager.CreateContentAsync(request.SiteId, channel.Id, contentId);
                }
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, channel.Id);
            }

            return new SubmitResult
            {
                ContentIds = contentIdList,
                IsIndex = channel.Knowledge && isChecked
            };
        }
    }
}