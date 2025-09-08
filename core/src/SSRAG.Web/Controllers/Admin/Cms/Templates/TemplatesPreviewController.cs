using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesPreviewController : ControllerBase
    {
        private const string Route = "cms/templates/templatesPreview";
        private const string RouteCache = "cms/templates/templatesPreview/actions/cache";

        private string CacheKey => _settingsManager.Cache.GetClassKey(typeof(TemplatesPreviewController));

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public TemplatesPreviewController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IParseManager parseManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public string Content { get; set; }
        }

        public class CacheRequest
        {
            public int SiteId { get; set; }
            public string Content { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public TemplateType TemplateType { get; set; }
            public int ChannelId { get; set; }
            public string Content { get; set; }
        }
    }
}
