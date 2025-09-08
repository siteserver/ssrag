using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsChannelGroupController : ControllerBase
    {
        private const string Route = "cms/settings/settingsChannelGroup";
        private const string RouteDelete = "cms/settings/settingsChannelGroup/actions/delete";
        private const string RouteOrder = "cms/settings/settingsChannelGroup/actions/order";

        private readonly IAuthManager _authManager;
        private readonly IChannelGroupRepository _channelGroupRepository;

        public SettingsChannelGroupController(
            IAuthManager authManager,
            IChannelGroupRepository channelGroupRepository
        )
        {
            _authManager = authManager;
            _channelGroupRepository = channelGroupRepository;
        }

        public class GetResult
        {
            public IEnumerable<ChannelGroup> Groups { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string GroupName { get; set; }
        }

        public class OrderRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public bool IsUp { get; set; }
            public int Rows { get; set; }
        }
    }
}
