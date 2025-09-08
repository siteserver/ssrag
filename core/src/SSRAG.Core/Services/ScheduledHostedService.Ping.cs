using System;
using System.Threading.Tasks;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class ScheduledHostedService
    {
        private async Task PingAsync(ScheduledTask task)
        {
            if (!string.IsNullOrEmpty(task.PingHost))
            {
                var host = PageUtils.RemoveProtocolFromUrl(task.PingHost);
                await _ping.SendPingAsync(task.PingHost, (int)TimeSpan.FromSeconds(5).TotalMilliseconds);
            }
        }
    }
}
