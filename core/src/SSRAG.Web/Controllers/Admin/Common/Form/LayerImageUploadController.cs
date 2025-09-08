using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageUploadController : ControllerBase
    {
        private const string Route = "common/form/layerImageUpload";
        private const string RouteUpload = "common/form/layerImageUpload/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly IStorageManager _storageManager;
        private readonly ISiteRepository _siteRepository;

        public LayerImageUploadController(
            IPathManager pathManager,
            IStorageManager storageManager,
            ISiteRepository siteRepository
        )
        {
            _pathManager = pathManager;
            _storageManager = storageManager;
            _siteRepository = siteRepository;
        }

        public class SubmitRequest : Options
        {
            public string UserName { get; set; }
            public int SiteId { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class SubmitResult
        {
            public string ImageUrl { get; set; }
            public string ImageVirtualUrl { get; set; }
            public string PreviewUrl { get; set; }
            public string PreviewVirtualUrl { get; set; }
        }

        public class Options
        {
            public bool IsEditor { get; set; }
            public bool IsThumb { get; set; }
            public int ThumbWidth { get; set; }
            public int ThumbHeight { get; set; }
            public bool IsLinkToOriginal { get; set; }
        }

        private static Options GetOptions(Site site)
        {
            return TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerImageUploadController)), new Options
            {
                IsEditor = false,
                IsThumb = false,
                ThumbWidth = 1024,
                ThumbHeight = 1024,
                IsLinkToOriginal = true
            });
        }

        private static void SetOptions(Site site, Options options)
        {
            site.Set(nameof(LayerImageUploadController), TranslateUtils.JsonSerialize(options));
        }
    }
}
