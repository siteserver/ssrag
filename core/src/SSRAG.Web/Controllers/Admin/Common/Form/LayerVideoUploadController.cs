using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerVideoUploadController : ControllerBase
    {
        private const string Route = "common/form/layerVideoUpload";

        private readonly IPathManager _pathManager;
        private readonly IVodManager _vodManager;
        private readonly IStorageManager _storageManager;
        private readonly ISiteRepository _siteRepository;

        public LayerVideoUploadController(
            IPathManager pathManager,
            IVodManager vodManager,
            IStorageManager storageManager,
            ISiteRepository siteRepository
        )
        {
            _pathManager = pathManager;
            _vodManager = vodManager;
            _storageManager = storageManager;
            _siteRepository = siteRepository;
        }

        public class Options
        {
            public bool IsChangeFileName { get; set; }
        }

        public class GetResult : Options
        {
            public bool IsCloudVod { get; set; }
            public string VideoUploadExtensions { get; set; }
        }

        public class SubmitRequest : Options
        {
            public int SiteId { get; set; }
        }

        public class SubmitResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public string CoverUrl { get; set; }
            public string PlayUrl { get; set; }
            public string VirtualUrl { get; set; }
        }
    }
}
