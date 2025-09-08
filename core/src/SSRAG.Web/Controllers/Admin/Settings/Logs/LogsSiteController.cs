using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsSiteController : ControllerBase
    {
        private const string Route = "settings/logsSite";
        private const string RouteExport = "settings/logsSite/actions/export";
        private const string RouteDelete = "settings/logsSite/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISiteLogRepository _siteLogRepository;

        public LogsSiteController(IAuthManager authManager, IPathManager pathManager, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, ISiteLogRepository siteLogRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _siteLogRepository = siteLogRepository;
        }

        public class SearchRequest : PageRequest
        {
            public List<int> SiteIds { get; set; }
            public string LogType { get; set; }
            public string UserName { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class SiteLogResult : SiteLog
        {
            public string AdminName { get; set; }
            public string AdminUuid { get; set; }
            public string WebUrl { get; set; }
            public string SiteName { get; set; }
        }

        public class SiteLogPageResult : PageResult<SiteLogResult>
        {
            public IEnumerable<Site> Sites { get; set; }
        }

        private async Task<SiteLogPageResult> GetResultsAsync(SearchRequest request)
        {
            var sites = await _siteRepository.GetSitesAsync();

            var adminId = 0;
            if (!string.IsNullOrEmpty(request.UserName))
            {
                var admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
                if (admin == null)
                {
                    return new SiteLogPageResult
                    {
                        Items = new List<SiteLogResult>(),
                        Count = 0,
                        Sites = sites
                    };
                }
                adminId = admin.Id;
            }

            var siteIdList = await _siteRepository.GetSiteIdsAsync();
            var siteIds = request.SiteIds;
            if (siteIds == null || siteIds.Count == 0)
            {
                siteIds = siteIdList;
            }

            var count = await _siteLogRepository.GetCountAsync(siteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo);
            var siteLogs = await _siteLogRepository.GetAllAsync(siteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            var logTasks = siteLogs.Select(async x =>
            {
                var site = await _siteRepository.GetAsync(x.SiteId);
                var admin = await _administratorRepository.GetByUserIdAsync(x.AdminId);
                var adminName = admin != null ? _administratorRepository.GetDisplay(admin) : string.Empty;
                var adminUuid = admin != null ? admin.Uuid : string.Empty;
                var log = new SiteLogResult
                {
                    Id = x.Id,
                    SiteId = x.SiteId,
                    ChannelId = x.ChannelId,
                    ContentId = x.ContentId,
                    AdminId = x.AdminId,
                    IpAddress = x.IpAddress,
                    Action = x.Action,
                    Summary = x.Summary,
                    SiteName = site.SiteName,
                    CreatedDate = x.CreatedDate,
                    AdminName = adminName,
                    AdminUuid = adminUuid,
                    WebUrl = _pathManager.GetWebUrl(site)
                };
                return log;
            });
            var logs = await Task.WhenAll(logTasks);

            return new SiteLogPageResult
            {
                Items = logs,
                Count = count,
                Sites = sites
            };
        }
    }
}
