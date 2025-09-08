using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class SpecialRepository : ISpecialRepository
    {
        private readonly Repository<Special> _repository;
        private readonly ISettingsManager _settingsManager;

        public SpecialRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Special>(settingsManager.Database, settingsManager.Cache);
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Special special)
        {
            var specialId = await _repository.InsertAsync(special, Q
                .CachingRemove(CacheKey(special.SiteId))
            );
            return specialId;
        }

        public async Task UpdateAsync(Special special)
        {
            await _repository.UpdateAsync(special, Q
                .CachingRemove(CacheKey(special.SiteId))
            );
        }

        public async Task DeleteAsync(int siteId, int specialId)
        {
            if (specialId <= 0) return;

            await _repository.DeleteAsync(specialId, Q
                .CachingRemove(CacheKey(siteId))
            );
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(Special.SiteId), siteId)
                .CachingRemove(CacheKey(siteId))
            );
        }

        public async Task<bool> IsTitleExistsAsync(int siteId, string title)
        {
            var specials = await GetSpecialsAsync(siteId);
            return specials.Exists(x => x.Url == title);
        }

        public async Task<bool> IsUrlExistsAsync(int siteId, string url)
        {
            var specials = await GetSpecialsAsync(siteId);
            return specials.Exists(x => x.Url == url);
        }

        public async Task<List<Special>> GetSpecialsAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Special.SiteId), siteId)
                .OrderByDesc(nameof(Special.Id))
                .CachingGet(CacheKey(siteId))
            );
        }

        private string CacheKey(int siteId) => _settingsManager.Cache.GetListKey(TableName, siteId);

        public async Task<Special> GetSpecialAsync(int siteId, int specialId)
        {
            var specials = await GetSpecialsAsync(siteId);
            return specials.FirstOrDefault(x => x.Id == specialId);
        }

        public async Task<string> GetTitleAsync(int siteId, int specialId)
        {
            var special = await GetSpecialAsync(siteId, specialId);
            return special != null ? special.Title : string.Empty;
        }

        public async Task<List<int>> GetAllSpecialIdsAsync(int siteId)
        {
            var specials = await GetSpecialsAsync(siteId);
            return specials.Select(x => x.Id).ToList();
        }

        public async Task<int> GetSpecialIdByTitleAsync(int siteId, string title)
        {
            var specials = await GetSpecialsAsync(siteId);
            var special = specials.FirstOrDefault(x => x.Title == title);
            return special?.Id ?? 0;
        }
    }
}
