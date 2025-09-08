using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageSelectController : ControllerBase
    {
        private const string Route = "common/form/layerImageSelect";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public LayerImageSelectController(
            IAuthManager authManager,
            IPathManager pathManager,
            ISiteRepository siteRepository
        )
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetRequest : SiteRequest
        {
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class Image
        {
            public string ImageUrl { get; set; }
            public string VirtualUrl { get; set; }
        }

        public class GetResult
        {
            public List<Image> Images { get; set; }
            public int Count { get; set; }
        }
    }
}
