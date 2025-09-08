using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class RedirectController : ControllerBase
    {
        private const string Route = "redirect";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public RedirectController(IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public class SubmitRequest : ChannelRequest
        {
            public int ContentId { get; set; }
            public int FileTemplateId { get; set; }
            public int SpecialId { get; set; }
            public bool IsLocal { get; set; }
        }
    }
}