﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Repositories
{
    public partial class TemplateRepository : ITemplateRepository
    {
        private readonly Repository<Template> _repository;
        private readonly ISettingsManager _settingsManager;

        public TemplateRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Template>(settingsManager.Database, settingsManager.Cache);
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Template template)
        {
            if (template.DefaultTemplate)
            {
                var defaultTemplate = await GetDefaultTemplateAsync(template.SiteId, template.TemplateType);
                if (defaultTemplate != null)
                {
                    defaultTemplate.DefaultTemplate = false;
                    await _repository.UpdateAsync(defaultTemplate, Q
                        .CachingRemove(GetListKey(template.SiteId))
                        .CachingRemove(GetEntityKey(defaultTemplate.Id))
                    );
                }
            }

            template.Id = await _repository.InsertAsync(template, Q
                .CachingRemove(GetListKey(template.SiteId))
            );

            return template.Id;
        }

        public async Task UpdateAsync(Template template)
        {
            var original = await GetAsync(template.Id);
            if (original.DefaultTemplate != template.DefaultTemplate && template.DefaultTemplate)
            {
                var defaultTemplate = await GetDefaultTemplateAsync(template.SiteId, template.TemplateType);
                if (defaultTemplate != null)
                {
                    defaultTemplate.DefaultTemplate = false;
                    await _repository.UpdateAsync(defaultTemplate, Q
                        .CachingRemove(GetListKey(template.SiteId))
                        .CachingRemove(GetEntityKey(defaultTemplate.Id))
                    );
                }
            }

            await _repository.UpdateAsync(template, Q
                .CachingRemove(GetListKey(template.SiteId))
                .CachingRemove(GetEntityKey(template.Id))
            );
        }

        public async Task SetDefaultAsync(int templateId)
        {
            var template = await GetAsync(templateId);

            var defaultTemplate = await GetDefaultTemplateAsync(template.SiteId, template.TemplateType);
            if (defaultTemplate != null && defaultTemplate.Id != templateId)
            {
                defaultTemplate.DefaultTemplate = false;
                await _repository.UpdateAsync(defaultTemplate, Q
                    .CachingRemove(GetListKey(template.SiteId))
                    .CachingRemove(GetEntityKey(defaultTemplate.Id))
                );
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Template.DefaultTemplate), true)
                .Where(nameof(Template.Id), templateId)
                .CachingRemove(GetListKey(template.SiteId))
                .CachingRemove(GetEntityKey(template.Id))
            );
        }

        public async Task DeleteAsync(IPathManager pathManager, Site site, int templateId)
        {
            var template = await GetAsync(templateId);
            var filePath = pathManager.GetTemplateFilePath(site, template);

            await _repository.DeleteAsync(templateId, Q
                .CachingRemove(GetListKey(template.SiteId))
                .CachingRemove(GetEntityKey(template.Id))
            );
            FileUtils.DeleteFileIfExists(filePath);
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(Template.SiteId), siteId)
                .CachingRemove(GetListKey(siteId))
            );
        }

        public async Task CreateDefaultTemplateAsync(int siteId)
        {
            await InsertAsync(new Template
            {
                Id = 0,
                SiteId = siteId,
                TemplateName = "系统首页模板",
                TemplateType = TemplateType.IndexPageTemplate,
                RelatedFileName = "T_系统首页模板.html",
                CreatedFileFullName = "@/index.html",
                CreatedFileExtName = ".html",
                DefaultTemplate = true
            });

            await InsertAsync(new Template
            {
                Id = 0,
                SiteId = siteId,
                TemplateName = "系统栏目模板",
                TemplateType = TemplateType.ChannelTemplate,
                RelatedFileName = "T_系统栏目模板.html",
                CreatedFileFullName = "index.html",
                CreatedFileExtName = ".html",
                DefaultTemplate = true
            });

            await InsertAsync(new Template
            {
                Id = 0,
                SiteId = siteId,
                TemplateName = "系统内容模板",
                TemplateType = TemplateType.ContentTemplate,
                RelatedFileName = "T_系统内容模板.html",
                CreatedFileFullName = "index.html",
                CreatedFileExtName = ".html",
                DefaultTemplate = true
            });
        }
    }
}
