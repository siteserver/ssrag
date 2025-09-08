using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public class StatRepository : IStatRepository
    {
        private readonly Repository<Stat> _repository;

        public StatRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Stat>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task AddCountAsync(StatType statType)
        {
            await AddCountAsync(statType, 0, string.Empty);
        }

        public async Task AddCountAsync(StatType statType, int siteId)
        {
            await AddCountAsync(statType, siteId, string.Empty);
        }

        public async Task AddCountAsync(StatType statType, int siteId, string adminName)
        {
            var lowerDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var higherDate = lowerDate.AddDays(1);

            var query = Q
                .Where(nameof(Stat.StatType), statType.GetValue())
                .WhereBetween(nameof(Stat.CreatedDate), lowerDate, higherDate);

            if (siteId > 0)
            {
                query.Where(nameof(Stat.SiteId), siteId);
            }
            if (!string.IsNullOrEmpty(adminName))
            {
                query.Where(nameof(Stat.AdminName), adminName);
            }

            if (await _repository.ExistsAsync(query))
            {
                await _repository.IncrementAsync(nameof(Stat.Count), query);
            }
            else
            {
                await _repository.InsertAsync(new Stat
                {
                    StatType = statType,
                    SiteId = siteId,
                    AdminName = adminName,
                    Count = 1
                });
            }
        }

        public async Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate, StatType statType)
        {
            return await GetStatsAsync(lowerDate, higherDate, statType, 0, string.Empty);
        }

        public async Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate, StatType statType, int siteId)
        {
            return await GetStatsAsync(lowerDate, higherDate, statType, siteId, string.Empty);
        }

        public async Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate, StatType statType, int siteId, string adminName)
        {
            var query = Q
                .Where(nameof(Stat.StatType), statType.GetValue())
                .WhereBetween(nameof(Stat.CreatedDate), lowerDate, higherDate.AddDays(1));

            if (siteId > 0)
            {
                query.Where(nameof(Stat.SiteId), siteId);
            }
            if (!string.IsNullOrEmpty(adminName))
            {
                query.Where(nameof(Stat.AdminName), adminName);
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task<List<Stat>> GetStatsWithAdminIdAsync(DateTime lowerDate, DateTime higherDate, StatType statType, int siteId)
        {
            var query = Q
                .Where(nameof(Stat.StatType), statType.GetValue())
                .WhereNotNullOrEmpty(nameof(Stat.AdminName))
                .WhereBetween(nameof(Stat.CreatedDate), lowerDate, higherDate.AddDays(1));

            if (siteId > 0)
            {
                query.Where(nameof(Stat.SiteId), siteId);
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(Stat.SiteId), siteId)
            );
        }
    }
}
