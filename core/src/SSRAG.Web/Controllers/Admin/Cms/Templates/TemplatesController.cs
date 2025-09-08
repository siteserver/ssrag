﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesController : ControllerBase
    {
        private const string Route = "cms/templates/templates";
        private const string RouteDelete = "cms/templates/templates/actions/delete";
        private const string RouteCreate = "cms/templates/templates/actions/create";
        private const string RouteCopy = "cms/templates/templates/actions/copy";
        private const string RouteDefault = "cms/templates/templates/actions/default";
        private const string RouteExport = "cms/templates/templates/actions/export";
        private const string RouteImport = "cms/templates/templates/actions/import";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateRepository _templateRepository;

        public TemplatesController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IPathManager pathManager,
            ICreateManager createManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            ITemplateRepository templateRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateRepository = templateRepository;
        }

        public class GetResult
        {
            public List<Cascade<int>> Channels { get; set; }
            public IEnumerable<Template> Templates { get; set; }
        }

        public class TemplateRequest
        {
            public int SiteId { get; set; }
            public int TemplateId { get; set; }
        }

        public class ImportRequest
        {
            public int SiteId { get; set; }
            public TemplateType TemplateType { get; set; }
        }

        private async Task<ActionResult<GetResult>> GetResultAsync(Site site)
        {
            var channels = new List<Channel>();
            var children = await _channelRepository.GetCascadeChildrenAsync(site, site.Id,
                async summary =>
                {
                    var entity = await _channelRepository.GetAsync(summary.Id);
                    channels.Add(entity);
                    return new
                    {
                        entity.ChannelTemplateId,
                        entity.ContentTemplateId
                    };
                });

            var summaries = await _templateRepository.GetSummariesAsync(site.Id);
            var templates = new List<Template>();
            foreach (var summary in summaries)
            {
                var original = await _templateRepository.GetAsync(summary.Id);
                var template = original.Clone<Template>();

                template.Set("useCount", _channelRepository.GetTemplateUseCount(site.Id, template.Id, template.TemplateType, template.DefaultTemplate, channels));

                if (template.TemplateType == TemplateType.IndexPageTemplate)
                {
                    template.Set("url", _pathManager.ParseSiteUrl(site, template.CreatedFileFullName, false));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.ChannelTemplate)
                {
                    template.Set("channelIds", _channelRepository.GetChannelIdsByTemplateId(true, template.Id, channels));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.ContentTemplate)
                {
                    template.Set("channelIds", _channelRepository.GetChannelIdsByTemplateId(false, template.Id, channels));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.FileTemplate)
                {
                    template.Set("url", _pathManager.ParseSiteUrl(site, template.CreatedFileFullName, false));
                    templates.Add(template);
                }
            }

            return new GetResult
            {
                Channels = children,
                Templates = templates
            };
        }
    }
}
