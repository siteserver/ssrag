using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesController : ControllerBase
    {
        private const string Route = "settings/sites";
        private const string RouteUpdate = "settings/sites/actions/update";
        private const string RouteDelete = "settings/sites/actions/delete";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly IFormRepository _formRepository;
        private readonly IFormDataRepository _formDataRepository;
        private readonly IRelatedFieldRepository _relatedFieldRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly IStatRepository _statRepository;
        private readonly ITemplateLogRepository _templateLogRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITranslateRepository _translateRepository;

        public SitesController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITableStyleRepository tableStyleRepository, IChannelGroupRepository channelGroupRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository, IContentCheckRepository contentCheckRepository, IFormRepository formRepository, IFormDataRepository formDataRepository, IRelatedFieldRepository relatedFieldRepository, IRelatedFieldItemRepository relatedFieldItemRepository, ISitePermissionsRepository sitePermissionsRepository, ISpecialRepository specialRepository, IStatRepository statRepository, ITemplateLogRepository templateLogRepository, ITemplateRepository templateRepository, ITranslateRepository translateRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _tableStyleRepository = tableStyleRepository;
            _channelGroupRepository = channelGroupRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
            _contentCheckRepository = contentCheckRepository;
            _formRepository = formRepository;
            _formDataRepository = formDataRepository;
            _relatedFieldRepository = relatedFieldRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _specialRepository = specialRepository;
            _statRepository = statRepository;
            _templateLogRepository = templateLogRepository;
            _templateRepository = templateRepository;
            _translateRepository = translateRepository;
        }

        public class GetResult
        {
            public IEnumerable<SiteType> SiteTypes { get; set; }
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
            public List<string> TableNames { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string SiteDir { get; set; }
            public bool DeleteFiles { get; set; }
        }

        public class EditRequest : SiteRequest
        {
            public string SiteDir { get; set; }
            public string SiteName { get; set; }
            public SiteType SiteType { get; set; }
            public int Taxis { get; set; }
            public TableRule TableRule { get; set; }
            public string TableChoose { get; set; }
            public string TableHandWrite { get; set; }
        }

        public class SitesResult
        {
            public List<Site> Sites { get; set; }
        }
    }
}
