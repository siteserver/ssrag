using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Configuration;
using SSRAG.Models;

namespace SSRAG.Core.Repositories
{
    public partial class SiteRepository
    {
        private string GetEntityKey(int siteId)
        {
            return _settingsManager.Cache.GetEntityKey(_repository.TableName, siteId);
        }

        private string GetListKey()
        {
            return _settingsManager.Cache.GetListKey(_repository.TableName);
        }

        public async Task<Site> GetAsync(int siteId)
        {
            if (siteId <= 0) return null;

            var site = await _repository.GetAsync(siteId, Q
                .CachingGet(GetEntityKey(siteId))
            );

            return site;
        }

        private async Task<List<SiteSummary>> GetSummariesAsync()
        {
            return await _repository.GetAllAsync<SiteSummary>(Q
                .Select(
                    nameof(Site.Id),
                    nameof(Site.SiteName),
                    nameof(Site.SiteType),
                    nameof(Site.IconUrl),
                    nameof(Site.SiteDir),
                    nameof(Site.Description),
                    nameof(Site.TableName),
                    nameof(Site.Root),
                    nameof(Site.Disabled),
                    nameof(Site.Taxis))
                .WhereNot(nameof(Site.Id), 0)
                .OrderBy(nameof(Site.Taxis), nameof(Site.Id))
                .CachingGet(GetListKey())
            );
        }
    }
}
