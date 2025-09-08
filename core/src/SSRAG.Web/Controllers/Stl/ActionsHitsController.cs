using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Repositories;

namespace SSRAG.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsHitsController : ControllerBase
    {
        private readonly IContentRepository _contentRepository;

        public ActionsHitsController(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public bool AutoIncrease { get; set; }
        }
    }
}
