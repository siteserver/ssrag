using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IScheduledTaskRepository : IRepository
    {
        Task<ScheduledTask> GetAsync(int id);

        Task<List<ScheduledTask>> GetAllAsync();

        Task<ScheduledTask> GetNextAsync();

        Task<int> InsertAsync(ScheduledTask task);

        Task<int> InsertPublishAsync(Content content, DateTime scheduledDate);

        Task<ScheduledTask> GetPublishAsync(int siteId, int channelId, int contentId);

        Task UpdateAsync(ScheduledTask task);

        Task<bool> DeleteAsync(int id);
    }
}