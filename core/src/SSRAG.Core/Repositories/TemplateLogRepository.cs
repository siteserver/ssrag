﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public class TemplateLogRepository : ITemplateLogRepository
    {
        private readonly Repository<TemplateLog> _repository;
        private readonly IAdministratorRepository _administratorRepository;

        public TemplateLogRepository(ISettingsManager settingsManager, IAdministratorRepository administratorRepository)
        {
            _repository = new Repository<TemplateLog>(settingsManager.Database, settingsManager.Cache);
            _administratorRepository = administratorRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(TemplateLog log)
        {
            await _repository.InsertAsync(log);
        }

        public async Task<string> GetTemplateContentAsync(int logId)
        {
            return await _repository.GetAsync<string>(Q.Select(nameof(TemplateLog.TemplateContent))
                .Where(nameof(TemplateLog.Id), logId)
            );
        }

        public async Task<List<KeyValuePair<int, string>>> GetLogIdWithNameListAsync(int siteId, int templateId)
        {
            var list = await _repository.GetAllAsync(Q
                .Where(nameof(TemplateLog.TemplateId), templateId)
                .OrderByDesc(nameof(TemplateLog.Id))
            );

            var pairs = new List<KeyValuePair<int, string>>();
            foreach (var templateLog in list)
            {
                var display = await _administratorRepository.GetDisplayAsync(templateLog.AdminName);
                pairs.Add(new KeyValuePair<int, string>(templateLog.Id,
                    $"修订时间：{DateUtils.GetDateAndTimeString(templateLog.CreatedDate)}，修订人：{display}，字符数：{templateLog.ContentLength}"));
            }

            return pairs;
        }

        public async Task DeleteAsync(int logId)
        {
            await _repository.DeleteAsync(logId);
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(TemplateLog.SiteId), siteId)
            );
        }
    }
}
