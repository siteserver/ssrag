using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Core.Repositories
{
    public partial class SiteRepository
    {
        public async Task<List<Site>> GetSitesAsync()
        {
            var sites = new List<Site>();

            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                sites.Add(await GetAsync(summary.Id));
            }
            return sites;
        }

        public async Task<List<Site>> GetSitesAsync(params SiteType[] siteTypes)
        {
            var sites = new List<Site>();
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries.Where(x => siteTypes.Contains(x.SiteType)))
            {
                sites.Add(await GetAsync(summary.Id));
            }
            return sites;
        }

        private async Task<SiteSummary> GetCacheAsync(int siteId)
        {
            var summaries = await GetSummariesAsync();
            return summaries.FirstOrDefault(x => x.Id == siteId);
        }

        public async Task<string> GetTableNameAsync(int siteId)
        {
            var site = await GetAsync(siteId);
            return site?.TableName;
        }

        public async Task<Site> GetSiteBySiteNameAsync(string siteName)
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (StringUtils.EqualsIgnoreCase(summary.SiteName, siteName))
                {
                    return await GetAsync(summary.Id);
                }
            }
            return null;
        }

        public async Task<Site> GetSiteByIsRootAsync()
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (summary.Root)
                {
                    return await GetAsync(summary.Id);
                }
            }
            return null;
        }

        public async Task<bool> IsRootExistsAsync()
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (summary.Root)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<Site> GetSiteByDirectoryAsync(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                var summaries = await GetSummariesAsync();
                foreach (var summary in summaries)
                {
                    if (summary.Root)
                    {
                        return await GetAsync(summary.Id);
                    }
                }
                return null;
            }

            return await GetSiteAsync(directory.Trim('/'));
        }

        private async Task<Site> GetSiteAsync(string siteDir)
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (StringUtils.EqualsIgnoreCase(summary.SiteDir, siteDir))
                {
                    return await GetAsync(summary.Id);
                }
            }
            return null;
        }

        public async Task<List<int>> GetSiteIdsAsync()
        {
            var summaries = await GetSummariesAsync();
            return summaries.Select(x => x.Id).ToList();
        }

        private void AddSiteIdList(List<int> dataSource, Site site, Hashtable parentWithChildren, int level)
        {
            dataSource.Add(site.Id);

            if (parentWithChildren[site.Id] != null)
            {
                var children = (List<Site>)parentWithChildren[site.Id];
                level++;

                var list = children.OrderBy(child => child.Taxis == 0 ? int.MaxValue : child.Taxis).ToList();

                foreach (var subSite in list)
                {
                    AddSiteIdList(dataSource, subSite, parentWithChildren, level);
                }
            }
        }

        public async Task<List<string>> GetSiteTableNamesAsync()
        {
            return await GetTableNamesAsync(true, false);
        }

        public async Task<List<string>> GetAllTableNamesAsync()
        {
            return await GetTableNamesAsync(true, true);
        }

        private async Task<List<string>> GetTableNamesAsync(bool includeSiteTables, bool includePluginTables)
        {
            var tableNames = new List<string>();

            if (includeSiteTables)
            {
                var summaries = await GetSummariesAsync();
                foreach (var summary in summaries)
                {
                    if (!string.IsNullOrEmpty(summary.TableName) && !ListUtils.ContainsIgnoreCase(tableNames, summary.TableName))
                    {
                        tableNames.Add(summary.TableName);
                    }
                }
            }

            if (includePluginTables)
            {
                var pluginTableNames = _settingsManager.GetContentTableNames();
                foreach (var pluginTableName in pluginTableNames)
                {
                    if (!string.IsNullOrEmpty(pluginTableName) && !ListUtils.ContainsIgnoreCase(tableNames, pluginTableName))
                    {
                        tableNames.Add(pluginTableName);
                    }
                }
            }

            return tableNames;
        }

        public async Task<List<string>> GetTableNamesAsync(Site site)
        {
            var tableNames = new List<string> { site.TableName };
            var channelSummaries = await _channelRepository.GetSummariesAsync(site.Id);
            var pluginTableNames = _settingsManager.GetContentTableNames();
            foreach (var summary in channelSummaries)
            {
                if (!string.IsNullOrEmpty(summary.TableName))
                {
                    if (ListUtils.Contains(pluginTableNames, summary.TableName))
                    {
                        tableNames.Add(summary.TableName);
                    }
                }
            }
            return tableNames;
        }

        public async Task<int> GetIdByIsRootAsync()
        {
            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (summary.Root)
                {
                    return summary.Id;
                }
            }

            return 0;
        }

        private async Task<int> GetMaxTaxisAsync()
        {
            var summaries = await GetSummariesAsync();
            return summaries.Select(x => x.Taxis).DefaultIfEmpty().Max();
        }

        public async Task<string> GetSiteDirCascadingAsync(int siteId)
        {
            var siteDirs = new List<string>();
            var summaries = await GetSummariesAsync();
            GetSiteDirCascading(summaries, siteDirs, siteId);
            siteDirs.Reverse();
            return ListUtils.ToString(siteDirs, "/");
        }

        private void GetSiteDirCascading(List<SiteSummary> summaries, List<string> siteDirs, int siteId)
        {
            foreach (var summary in summaries)
            {
                if (summary.Id == siteId)
                {
                    if (!string.IsNullOrEmpty(summary.SiteDir))
                    {
                        siteDirs.Add(summary.SiteDir);
                    }
                }
            }
        }

        public async Task<IList<string>> GetSiteDirsAsync()
        {
            var siteDirList = new List<string>();

            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (!summary.Root)
                {
                    siteDirList.Add(summary.SiteDir);
                }
            }

            return siteDirList;
        }

        public async Task<List<Select<int>>> GetSelectsAsync(List<int> includedSiteIds = null)
        {
            var selects = new List<Select<int>>();

            var summaries = await GetSummariesAsync();
            foreach (var summary in summaries)
            {
                if (includedSiteIds != null && !includedSiteIds.Contains(summary.Id))
                {
                    continue;
                }
                selects.Add(new Select<int>(summary.Id, summary.SiteName));
            }

            return selects;
        }
    }
}