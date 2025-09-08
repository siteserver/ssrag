using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Repositories
{
    public class ScheduledTaskRepository : IScheduledTaskRepository
    {
        private readonly Repository<ScheduledTask> _repository;
        private readonly ISettingsManager _settingsManager;

        public ScheduledTaskRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ScheduledTask>(settingsManager.Database, settingsManager.Cache);
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey => _settingsManager.Cache.GetClassKey(typeof(ScheduledTaskRepository));

        public async Task<ScheduledTask> GetAsync(int id)
        {
            var tasks = await GetAllAsync();
            return tasks.FirstOrDefault(t => t.Id == id);
        }

        public async Task<List<ScheduledTask>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderBy(nameof(ScheduledTask.Id))
                .CachingGet(CacheKey)
            );
        }

        public async Task<ScheduledTask> GetNextAsync()
        {
            var tasks = await GetAllAsync();
            var nextTask = tasks.Where(x => !x.IsDisabled && x.ScheduledDate.HasValue && x.ScheduledDate.Value < DateTime.Now).OrderBy(x => x.ScheduledDate).FirstOrDefault();
            return nextTask;
        }

        private static DateTime? CalcScheduledDate(ScheduledTask task)
        {
            var now = DateTime.Now;
            if (task.IsDisabled) return null;

            DateTime? date = null;
            if (task.TaskInterval == TaskInterval.Once)
            {
                if (task.StartDate < now) return null;
                date = task.StartDate;
            }
            else if (task.TaskInterval == TaskInterval.EveryHour)
            {
                if (task.LatestStartDate.HasValue)
                {
                    var ts = new TimeSpan(now.Ticks - task.LatestStartDate.Value.Ticks);
                    if (ts.Hours > task.Every)
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, now.Hour, task.StartDate.Minute, task.StartDate.Second);
                        if (date < now)
                        {
                            date = date.Value.AddHours(1);
                        }
                    }
                    else
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, task.LatestStartDate.Value.Hour, task.StartDate.Minute, task.StartDate.Second);
                        date = date.Value.AddHours(task.Every);
                    }
                }
                else
                {
                    if (task.StartDate < now)
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, now.Hour, task.StartDate.Minute, task.StartDate.Second);
                        if (date < now)
                        {
                            date = date.Value.AddHours(1);
                        }
                    }
                    else
                    {
                        date = task.StartDate;
                    }
                }
            }
            else if (task.TaskInterval == TaskInterval.EveryDay)
            {
                if (task.LatestStartDate.HasValue)
                {
                    var ts = new TimeSpan(now.Ticks - task.LatestStartDate.Value.Ticks);
                    if (ts.Days > task.Every)
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, task.StartDate.Hour, task.StartDate.Minute, task.StartDate.Second);
                        if (date < now)
                        {
                            date = date.Value.AddDays(1);
                        }
                    }
                    else
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, task.StartDate.Hour, task.StartDate.Minute, task.StartDate.Second);
                        date = date.Value.AddDays(task.Every);
                    }
                }
                else
                {
                    if (task.StartDate < now)
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, task.StartDate.Hour, task.StartDate.Minute, task.StartDate.Second);
                        if (date < now)
                        {
                            date = date.Value.AddDays(1);
                        }
                    }
                    else
                    {
                        date = task.StartDate;
                    }
                }
            }
            else if (task.TaskInterval == TaskInterval.EveryWeek)
            {
                var nowWeek = (int)now.DayOfWeek;
                if (task.Weeks != null && task.Weeks.Count > 0)
                {
                    date = new DateTime(now.Year, now.Month, now.Day, task.StartDate.Hour, task.StartDate.Minute, task.StartDate.Second);
                    for (var i = nowWeek; i <= 6 + nowWeek; i++)
                    {
                        var theWeek = i % 7;
                        if (task.Weeks.Contains(theWeek))
                        {
                            date = date.Value.AddDays(i - nowWeek);
                            break;
                        }
                    }
                }
            }
            return date;
        }

        public async Task<int> InsertAsync(ScheduledTask task)
        {
            task.ScheduledDate = CalcScheduledDate(task);
            return await _repository.InsertAsync(task, Q.CachingRemove(CacheKey));
        }

        public async Task<ScheduledTask> GetPublishAsync(int siteId, int channelId, int contentId)
        {
            var tasks = await GetAllAsync();
            return tasks.FirstOrDefault(t => t.PublishSiteId == siteId && t.PublishChannelId == channelId && t.PublishContentId == contentId);
        }

        public async Task<int> InsertPublishAsync(Content content, DateTime scheduledDate)
        {
            var task = await GetPublishAsync(content.SiteId, content.ChannelId, content.Id);
            if (task != null)
            {
                await DeleteAsync(task.Id);
            }

            task = new ScheduledTask
            {
                Title = TaskType.Publish.GetDisplayName(),
                TaskType = TaskType.Publish.GetValue(),
                TaskInterval = TaskInterval.Once,
                StartDate = DateTime.Now,
                IsNoticeSuccess = true,
                IsNoticeFailure = true,
                NoticeFailureCount = 0,
                IsNoticeMobile = true,
                NoticeMobile = string.Empty,
                IsDisabled = false,
                Timeout = 10,
                ScheduledDate = scheduledDate,
                PublishSiteId = content.SiteId,
                PublishChannelId = content.ChannelId,
                PublishContentId = content.Id,
            };
            return await _repository.InsertAsync(task, Q.CachingRemove(CacheKey));
        }

        public async Task UpdateAsync(ScheduledTask task)
        {
            task.ScheduledDate = CalcScheduledDate(task);
            await _repository.UpdateAsync(task, Q.CachingRemove(CacheKey));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id, Q.CachingRemove(CacheKey));
        }
    }
}
