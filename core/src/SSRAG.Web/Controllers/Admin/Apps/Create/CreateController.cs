using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Enums;
using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Web.Controllers.Admin.Apps.Create
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class CreateController : ControllerBase
    {
        public const string Route = "apps/create";
        private const string RouteProcess = "apps/create/actions/process";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ITemplateRepository _templateRepository;

        public CreateController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IContentRepository contentRepository, IAdministratorRepository administratorRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
            _administratorRepository = administratorRepository;
            _templateRepository = templateRepository;
        }

        public class SubmitRequest
        {
            public string Uuid { get; set; }
            public SiteType SiteType { get; set; }
            public string SiteTemplate { get; set; }
            public string SiteName { get; set; }
            public string Description { get; set; }
            public string IconUrl { get; set; }
            public bool Root { get; set; }
            public string SiteDir { get; set; }
            public CreateType CreateType { get; set; }
        }

        public class ProcessRequest
        {
            public string Uuid { get; set; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum CreateType
        {
            [DataEnum] CreateEmpty,
            [DataEnum] UseTemplate,
            [DataEnum] ImportTemplate,
        }
    }
}
