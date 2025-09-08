﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SSRAG.Datory;
using SqlKata;
using SSRAG.Core.StlParser.Models;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task<int> GetHitsAsync(int siteId, int channelId, int contentId)
        {
            var tableName = await _siteRepository.GetTableNameAsync(siteId);
            var repository = await GetRepositoryAsync(tableName);

            return await repository.GetAsync<int>(Q
                .Select(nameof(Content.Hits))
                .Where(nameof(Content.SiteId), siteId)
                .Where(nameof(Content.ChannelId), channelId)
                .Where(nameof(Content.Id), contentId)
            );
        }

        public async Task<int> GetMaxTaxisAsync(Site site, Channel channel, bool isTop)
        {
            var repository = await GetRepositoryAsync(site, channel);

            var maxTaxis = 0;
            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                var max = await repository.MaxAsync(nameof(Content.Taxis),
                    GetQuery(site.Id, channel.Id)
                        .Where(nameof(Content.Taxis), ">", TaxisIsTopStartValue)
                );
                if (max.HasValue)
                {
                    maxTaxis = max.Value;
                }

                if (maxTaxis < TaxisIsTopStartValue)
                {
                    maxTaxis = TaxisIsTopStartValue;
                }
            }
            else
            {
                var max = await repository.MaxAsync(nameof(Content.Taxis),
                    GetQuery(site.Id, channel.Id)
                    .Where(nameof(Content.Taxis), "<", TaxisIsTopStartValue)
                );
                if (max.HasValue)
                {
                    maxTaxis = max.Value;
                }
            }
            return maxTaxis;
        }

        private async Task<List<ContentSummary>> GetReferenceIdListAsync(string tableName, IEnumerable<int> contentIdList)
        {
            var repository = await GetRepositoryAsync(tableName);
            return await repository.GetAllAsync<ContentSummary>(Q
                .Select(nameof(Content.Id), nameof(Content.ChannelId))
                .Where(nameof(Content.ChannelId), ">", 0)
                .WhereIn(nameof(Content.ReferenceId), contentIdList)
            );
        }

        public async Task<int> GetFirstContentIdAsync(Site site, IChannelSummary channel)
        {
            var repository = await GetRepositoryAsync(site, channel);
            return await repository.GetAsync<int>(Q
                .Select(nameof(Content.Id))
                .Where(nameof(Content.ChannelId), channel.Id)
                .OrderByDesc(nameof(Content.Taxis), nameof(Content.Id))
            );
        }

        public async Task<List<int>> GetContentIdsBySameTitleAsync(Site site, Channel channel, string title)
        {
            var repository = await GetRepositoryAsync(site, channel);

            return await repository.GetAllAsync<int>(GetQuery(site.Id, channel.Id)
                .Select(nameof(Content.Id))
                .Where(nameof(Content.Title), title)
            );
        }

        public async Task<List<ContentSummary>> GetSummariesAsync(string tableName, Query query)
        {
            var repository = await GetRepositoryAsync(tableName);
            return await repository.GetAllAsync<ContentSummary>(query);
        }

        public async Task<Query> GetQueryByStlSearchAsync(IDatabaseManager databaseManager, bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string groups, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes, NameValueCollection form)
        {
            var query = Q.NewQuery();

            Site site = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                site = await _siteRepository.GetSiteBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                site = await _siteRepository.GetSiteByDirectoryAsync(siteDir);
            }
            if (site == null)
            {
                site = await _siteRepository.GetAsync(siteId);
            }

            var channelId = await _channelRepository.GetChannelIdAsync(siteId, siteId, channelIndex, channelName);
            var channel = await _channelRepository.GetAsync(channelId);

            if (isAllSites)
            {
                query.Where(nameof(Content.SiteId), ">", 0);
            }
            else if (!string.IsNullOrEmpty(siteIds))
            {
                query.WhereIn(nameof(Content.SiteId), ListUtils.GetIntList(siteIds));
            }
            else
            {
                query.Where(nameof(Content.SiteId), site.Id);
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                var channelIdList = new List<int>();
                foreach (var theChannelId in ListUtils.GetIntList(channelIds))
                {
                    var theChannel = await _channelRepository.GetAsync(theChannelId);
                    if (theChannel != null)
                    {
                        var theChannelIds = await _channelRepository.GetChannelIdsAsync(theChannel.SiteId, theChannel.Id, ScopeType.All);
                        channelIdList.AddRange(theChannelIds);
                    }
                }

                if (channelIdList.Count == 1)
                {
                    query.Where(nameof(Content.ChannelId), channelIdList[0]);
                }
                else
                {
                    query.WhereIn(nameof(Content.ChannelId), channelIdList);
                }
            }
            else if (channelId != siteId)
            {
                var channelIdList = await _channelRepository.GetChannelIdsAsync(siteId, channelId, ScopeType.All);

                if (channelIdList.Count == 1)
                {
                    query.Where(nameof(Content.ChannelId), channelIdList[0]);
                }
                else
                {
                    query.WhereIn(nameof(Content.ChannelId), channelIdList);
                }
            }

            if (!string.IsNullOrEmpty(groups))
            {
                query.Where(q =>
                {
                    foreach (var groupName in ListUtils.GetStringList(groups))
                    {
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            q
                                .OrWhere(nameof(Content.GroupNames), groupName)
                                .OrWhereLike(nameof(Content.GroupNames), $"{groupName},%")
                                .OrWhereLike(nameof(Content.GroupNames), $"%,{groupName},%")
                                .OrWhereLike(nameof(Content.GroupNames), $"%,{groupName}");
                        }
                    }
                    return q;
                });
            }

            var typeList = new List<string>();
            if (string.IsNullOrEmpty(type))
            {
                typeList.Add(nameof(Content.Title));
            }
            else
            {
                typeList = ListUtils.GetStringList(type);
            }

            var tableName = _channelRepository.GetTableName(site, channel);
            var columns = await databaseManager.GetTableColumnInfoListAsync(tableName, excludeAttributes);

            if (!string.IsNullOrEmpty(word))
            {
                query.Where(q =>
                    {
                        foreach (var attributeName in typeList)
                        {
                            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ListInfo.Tags)) || StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.TagNames)))
                            {
                                q
                                    //.OrWhere(nameof(Content.TagNames), word)
                                    .OrWhereLike(nameof(Content.TagNames), $"%{word}%");
                                //.OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $",{word}")
                                //.OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $",{word},")
                                //.OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $"{word},");
                            }
                            else
                            {
                                var column = columns.FirstOrDefault(x =>
                                    StringUtils.EqualsIgnoreCase(x.AttributeName, attributeName));

                                if (column != null && (column.DataType == DataType.VarChar || column.DataType == DataType.Text))
                                {
                                    q.OrWhereLike(column.AttributeName, $"%{word}%");
                                }
                                //else
                                //{
                                //    q.OrWhereLike(AttrExtendValues, $"%{attributeName}={word}%");
                                //}
                            }
                        }

                        return q;
                    }
                );
            }

            if (string.IsNullOrEmpty(dateAttribute))
            {
                dateAttribute = nameof(Content.AddDate);
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                query.Where(dateAttribute, ">=", dateFrom);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                query.Where(dateAttribute, "<=", dateTo);
            }
            if (!string.IsNullOrEmpty(since))
            {
                var sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));

                query.WhereBetween(dateAttribute, sinceDate, DateTime.Now);
            }


            //var styleInfoList = RelatedIdentities.GetTableStyleInfoList(site, channel.Id);

            foreach (string attributeName in form.Keys)
            {
                if (ListUtils.ContainsIgnoreCase(excludeAttributes, attributeName)) continue;
                if (string.IsNullOrEmpty(form[attributeName])) continue;

                var value = StringUtils.Trim(form[attributeName]);
                if (string.IsNullOrEmpty(value)) continue;

                if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ListInfo.Tags)) || StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.TagNames)))
                {
                    query.Where(q => q
                        .Where(nameof(Content.TagNames), word)
                        .OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $",{word}")
                        .OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $",{word},")
                        .OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $"{word},"));
                }
                else
                {
                    var column = columns.FirstOrDefault(x =>
                        StringUtils.EqualsIgnoreCase(x.AttributeName, attributeName));

                    if (column != null && (column.DataType == DataType.VarChar || column.DataType == DataType.Text))
                    {
                        query.WhereLike(column.AttributeName, $"%{value}%");
                    }
                    //else
                    //{
                    //    query.WhereLike(AttrExtendValues, $"%{attributeName}={value}%");
                    //}
                }
            }

            return query;
        }

        public async Task<string> GetNewContentTableNameAsync()
        {
            var name = DateTime.Now.ToString("yyyyMMdd");

            var i = 1;
            do
            {
                var tableName = $"siteserver_{name}_{i++}";
                if (!await _settingsManager.Database.IsTableExistsAsync(tableName))
                {
                    return tableName;
                }
            } while (true);
        }

        public async Task<string> CreateNewContentTableAsync()
        {
            var tableName = await GetNewContentTableNameAsync();
            var repository = new Repository<Content>(_settingsManager.Database);
            await CreateContentTableAsync(tableName, repository.TableColumns);
            return tableName;
        }

        public async Task CreateContentTableAsync(string tableName, List<TableColumn> columnInfoList)
        {
            var isDbExists = await _settingsManager.Database.IsTableExistsAsync(tableName);
            if (isDbExists) return;

            await _settingsManager.Database.CreateTableAsync(tableName, columnInfoList);
            // await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.Taxis)}", $"{nameof(Content.Taxis)} DESC");
            // await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.SiteId)}", $"{nameof(Content.SiteId)} DESC");
            // await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.ChannelId)}", $"{nameof(Content.ChannelId)} DESC");
            // await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.AddDate)}", $"{nameof(Content.AddDate)} DESC");
            // await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.SourceId)}", $"{nameof(Content.SourceId)} DESC");

            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.Taxis)}", nameof(Content.Taxis));
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.SiteId)}", nameof(Content.SiteId));
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.ChannelId)}", nameof(Content.ChannelId));
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.AddDate)}", nameof(Content.AddDate));
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.SourceId)}", nameof(Content.SourceId));
        }

        private async Task QueryWhereAsync(Query query, Site site, int channelId, bool isAllContents)
        {
            query.Where(nameof(Content.SiteId), site.Id);
            query.WhereNot(nameof(Content.SourceId), SourceManager.Preview);

            if (isAllContents)
            {
                var channelIdList = await _channelRepository.GetChannelIdsAsync(site.Id, channelId, ScopeType.All);
                query.WhereIn(nameof(Content.ChannelId), channelIdList);
            }
            else
            {
                query.Where(nameof(Content.ChannelId), channelId);
            }
        }
    }
}
