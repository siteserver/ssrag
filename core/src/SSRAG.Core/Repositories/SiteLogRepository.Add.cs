﻿using System;
using System.Threading.Tasks;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Core.Repositories
{
    public partial class SiteLogRepository
    {
        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, Administrator adminInfo, string ipAddress, string action, string summary)
        {
            var config = await _configRepository.GetAsync();
            if (!config.IsLogSite) return;

            if (siteId <= 0)
            {
                await _logRepository.AddAdminLogAsync(adminInfo, ipAddress, action, summary);
            }
            else
            {
                try
                {
                    await DeleteIfThresholdAsync();

                    if (!string.IsNullOrEmpty(action))
                    {
                        action = StringUtils.MaxLengthText(action, 250);
                    }
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = StringUtils.MaxLengthText(summary, 250);
                    }
                    if (channelId < 0)
                    {
                        channelId = -channelId;
                    }

                    var siteLogInfo = new SiteLog
                    {
                        Id = 0,
                        SiteId = siteId,
                        ChannelId = channelId,
                        ContentId = contentId,
                        AdminId = adminInfo.Id,
                        IpAddress = ipAddress,
                        Action = action,
                        Summary = summary
                    };

                    await InsertAsync(siteLogInfo);

                    await _administratorRepository.UpdateLastActivityDateAsync(adminInfo);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(ex);
                }
            }
        }

        public async Task AddSiteCreateLogAsync(int siteId, int channelId, int contentId, Administrator adminInfo, string ipAddress, string filePath)
        {
            var config = await _configRepository.GetAsync();
            if (!config.IsLogSite || !config.IsLogSiteCreate) return;

            try
            {
                await DeleteIfThresholdAsync();

                var siteLogInfo = new SiteLog
                {
                    Id = 0,
                    SiteId = siteId,
                    ChannelId = channelId,
                    ContentId = contentId,
                    AdminId = adminInfo.Id,
                    IpAddress = ipAddress,
                    Action = Constants.ActionsCreate,
                    Summary = filePath
                };

                await InsertAsync(siteLogInfo);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(ex);
            }
        }
    }
}
