using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Repositories;
using SSRAG.Services;
using System.Collections.Generic;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Apps
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AppsController : ControllerBase
    {
        private const string Route = "apps";
        private const string RouteClearCache = "apps/actions/clearCache";
        private const string RouteGetTemplates = "apps/actions/getTemplates";
        private const string RouteUploadTemplate = "apps/actions/uploadTemplate";
        private const string RouteDeleteCore = "apps/actions/deleteCore";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
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
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IFlowNodeRepository _flowNodeRepository;
        private readonly IFlowVariableRepository _flowVariableRepository;
        private readonly IFlowEdgeRepository _flowEdgeRepository;

        public AppsController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IPathManager pathManager,
            IDatabaseManager databaseManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository,
            ITableStyleRepository tableStyleRepository,
            IChannelGroupRepository channelGroupRepository,
            IContentGroupRepository contentGroupRepository,
            IContentTagRepository contentTagRepository,
            IContentCheckRepository contentCheckRepository,
            IFormRepository formRepository,
            IFormDataRepository formDataRepository,
            IRelatedFieldRepository relatedFieldRepository,
            IRelatedFieldItemRepository relatedFieldItemRepository,
            ISitePermissionsRepository sitePermissionsRepository,
            ISpecialRepository specialRepository,
            IStatRepository statRepository,
            ITemplateLogRepository templateLogRepository,
            ITemplateRepository templateRepository,
            ITranslateRepository translateRepository,
            IAdministratorRepository administratorRepository,
            IFlowNodeRepository flowNodeRepository,
            IFlowVariableRepository flowVariableRepository,
            IFlowEdgeRepository flowEdgeRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
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
            _administratorRepository = administratorRepository;
            _flowNodeRepository = flowNodeRepository;
            _flowVariableRepository = flowVariableRepository;
            _flowEdgeRepository = flowEdgeRepository;
        }

        public class GetTemplatesRequest
        {
            public SiteType SiteType { get; set; }
        }

        public class GetTemplatesResult
        {
            public List<SiteTemplate> SiteTemplates { get; set; }
        }

        public class DeleteCoreRequest : SiteRequest
        {
            public string SiteDir { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string SiteName { get; set; }
            public SiteType SiteType { get; set; }
            public bool Root { get; set; }
            public string SiteDir { get; set; }
            public string Description { get; set; }
            public string IconUrl { get; set; }
        }
    }
}