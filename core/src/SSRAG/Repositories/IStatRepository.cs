using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IStatRepository : IRepository
    {
        Task AddCountAsync(StatType statType);

        Task AddCountAsync(StatType statType, int siteId);

        Task AddCountAsync(StatType statType, int siteId, string adminName);

        Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate,
            StatType statType);

        Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate,
            StatType statType, int siteId);

        Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate,
            StatType statType, int siteId, string adminName);

        Task<List<Stat>> GetStatsWithAdminIdAsync(DateTime lowerDate, DateTime higherDate,
            StatType statType, int siteId);

        Task DeleteAllAsync(int siteId);
    }
}