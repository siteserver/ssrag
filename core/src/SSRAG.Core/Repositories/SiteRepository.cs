using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class SiteRepository : ISiteRepository
    {
        private readonly Repository<Site> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly IChannelRepository _channelRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ITemplateRepository _templateRepository;

        public SiteRepository(ISettingsManager settingsManager, IChannelRepository channelRepository, IAdministratorRepository administratorRepository, ITemplateRepository templateRepository)
        {
            _repository = new Repository<Site>(settingsManager.Database, settingsManager.Cache);
            _settingsManager = settingsManager;
            _channelRepository = channelRepository;
            _administratorRepository = administratorRepository;
            _templateRepository = templateRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        /// <summary>
        /// 添加后台发布节点
        /// </summary>
        public async Task<(int, string)> InsertSiteAsync(Channel channel, Site site, string adminName)
        {
            if (_settingsManager.MaxSites > 0)
            {
                var siteIds = await GetSiteIdsAsync();
                if (siteIds.Count >= _settingsManager.MaxSites)
                {
                    return (0, "站点数量已超过限制，无法创建新站点!");
                }
            }

            await _channelRepository.InsertChannelAsync(null, channel);

            site.Id = channel.Id;

            await InsertAsync(site);

            var adminEntity = await _administratorRepository.GetByUserNameAsync(adminName);
            await _administratorRepository.UpdateSiteIdAsync(adminEntity, channel.Id);

            channel.SiteId = site.Id;
            await _channelRepository.UpdateAsync(channel);

            await _templateRepository.CreateDefaultTemplateAsync(site.Id);

            return (channel.Id, string.Empty);
        }

        private async Task<int> InsertAsync(Site site)
        {
            site.Taxis = await GetMaxTaxisAsync() + 1;
            site.Id = await _repository.InsertAsync(site, Q
                .AllowIdentityInsert()
                .CachingRemove(GetListKey())
            );
            return site.Id;
        }

        public async Task DeleteAsync(int siteId)
        {
            await _repository.DeleteAsync(siteId, Q
                .CachingRemove(GetListKey(), GetEntityKey(siteId))
            );
        }

        public async Task ClearCacheAsync(int siteId)
        {
            await _settingsManager.Cache.RemoveAsync(GetListKey());
            if (siteId > 0)
            {
                await _settingsManager.Cache.RemoveAsync(GetEntityKey(siteId));
            }
        }

        public async Task UpdateAsync(Site site)
        {
            var cache = await GetCacheAsync(site.Id);
            if (site.Root != cache.Root)
            {
                await UpdateAllIsRootAsync();
            }

            await _repository.UpdateAsync(site, Q
                .CachingRemove(GetListKey(), GetEntityKey(site.Id))
            );
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.TableName), tableName)
                .Where(nameof(Site.Id), siteId)
                .CachingRemove(GetListKey(), GetEntityKey(siteId))
            );
        }

        private async Task UpdateAllIsRootAsync()
        {
            var cacheKeys = new List<string>
            {
                GetListKey()
            };
            var siteIds = await GetSiteIdsAsync();
            foreach (var siteId in siteIds)
            {
                cacheKeys.Add(GetEntityKey(siteId));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Site.Root), false)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task<List<KeyValuePair<int, Site>>> ParserGetSitesAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, TaxisType taxisType)
        {
            var sites = new List<Site>();
            var summaries = await GetSummariesAsync();

            Site site = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                site = await GetSiteBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                site = await GetSiteByDirectoryAsync(siteDir);
            }

            var siteIds = summaries.Select(x => x.Id).ToList();
            foreach (var siteId in siteIds)
            {
                sites.Add(await GetAsync(siteId));
            }

            sites = ParserOrder(sites, taxisType);
            if (startNum > 1 && totalNum > 0)
            {
                sites = sites.Skip(startNum - 1).Take(totalNum).ToList();
            }
            else if (startNum > 1)
            {
                sites = sites.Skip(startNum - 1).ToList();
            }
            else if (totalNum > 0)
            {
                sites = sites.Take(totalNum).ToList();
            }

            var list = new List<KeyValuePair<int, Site>>();
            var i = 0;
            foreach (var entity in sites)
            {
                list.Add(new KeyValuePair<int, Site>(i++, entity));
            }

            return list;
        }

        private static List<Site> ParserOrder(List<Site> sites, TaxisType taxisType)
        {
            if (taxisType == TaxisType.OrderById)
            {
                return sites.OrderBy(x => x.Id).ToList();
            }

            if (taxisType == TaxisType.OrderByIdDesc)
            {
                return sites.OrderByDescending(x => x.Id).ToList();
            }

            if (taxisType == TaxisType.OrderByAddDate)
            {
                return sites.OrderBy(x => x.CreatedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByAddDateDesc)
            {
                return sites.OrderByDescending(x => x.CreatedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByLastModifiedDate)
            {
                return sites.OrderBy(x => x.LastModifiedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByLastModifiedDateDesc)
            {
                return sites.OrderByDescending(x => x.LastModifiedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByTaxis)
            {
                return sites.OrderBy(x => x.Taxis).ToList();
            }

            if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                return sites.OrderByDescending(x => x.Taxis).ToList();
            }

            if (taxisType == TaxisType.OrderByRandom)
            {
                return sites.OrderBy(x => Guid.NewGuid()).ToList();
            }

            return sites.OrderBy(x => x.Taxis).ToList();
        }
    }
}
