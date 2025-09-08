﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Admin.Document.Channels
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ChannelsController : ControllerBase
    {
        private const string Route = "document/channels/channels";
        private const string RouteGet = "document/channels/channels/{siteId:int}/{channelId:int}";
        private const string RouteUpdate = "document/channels/channels/actions/update";
        private const string RouteDelete = "document/channels/channels/actions/delete";
        private const string RouteAppend = "document/channels/channels/actions/append";
        private const string RouteUpload = "document/channels/channels/actions/upload";
        private const string RouteImport = "document/channels/channels/actions/import";
        private const string RouteExport = "document/channels/channels/actions/export";
        private const string RouteDrop = "document/channels/channels/actions/drop";
        private const string RouteColumns = "document/channels/channels/actions/columns";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IDocumentRepository _documentRepository;

        public ChannelsController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IPathManager pathManager,
            ICreateManager createManager,
            IDatabaseManager databaseManager,
            IPluginManager pluginManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository,
            IChannelGroupRepository channelGroupRepository,
            ITemplateRepository templateRepository,
            ITableStyleRepository tableStyleRepository,
            IRelatedFieldItemRepository relatedFieldItemRepository,
            IDbCacheRepository dbCacheRepository,
            IDocumentRepository documentRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _channelGroupRepository = channelGroupRepository;
            _templateRepository = templateRepository;
            _tableStyleRepository = tableStyleRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
            _dbCacheRepository = dbCacheRepository;
            _documentRepository = documentRepository;
        }

        public class ChannelColumn
        {
            public string AttributeName { get; set; }
            public string DisplayName { get; set; }
            public InputType InputType { get; set; }
            public bool IsList { get; set; }
        }

        public class ColumnsRequest : ChannelRequest
        {
            public List<string> AttributeNames { get; set; }
        }

        public class LinkTo
        {
            public List<int> ChannelIds { get; set; }

            public int ContentId { get; set; }

            public string ContentTitle { get; set; }
        }

        public class ListResult
        {
            public Cascade<int> Channel { get; set; }
            public IEnumerable<string> IndexNames { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<Template> ChannelTemplates { get; set; }
            public IEnumerable<Template> ContentTemplates { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public int CommandsWidth { get; set; }
            public bool IsTemplateEditable { get; set; }
            public IEnumerable<Select<string>> LinkTypes { get; set; }
            public IEnumerable<Select<string>> TaxisTypes { get; set; }
            public string SiteUrl { get; set; }
            public List<Menu> ChannelMenus { get; set; }
            public List<Menu> ChannelsMenus { get; set; }
        }

        public class GetResult
        {
            public Entity Entity { get; set; }
            public IEnumerable<TableStyle> Styles { get; set; }
            public Dictionary<int, List<Dto.Cascade<int>>> RelatedFields { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
            public LinkTo LinkTo { get; set; }
        }

        public class ImportRequest : ChannelRequest
        {
            public string FileName { get; set; }
            public bool IsOverride { get; set; }
            public bool IsContents { get; set; }
        }

        public class DropRequest : SiteRequest
        {
            public int SourceId { get; set; }
            public int TargetId { get; set; }
            public string DropType { get; set; }
        }

        public class ExportRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
            public bool IsContents { get; set; }
            public bool IsFileImages { get; set; }
            public bool IsFileAttaches { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int ChannelId { get; set; }
            public string ChannelName { get; set; }
            public bool DeleteFiles { get; set; }
        }

        public class AppendRequest : SiteRequest
        {
            public int ParentId { get; set; }
            public bool IsKnowledge { get; set; }
            public string Channels { get; set; }
        }

        public class UpdateRequest : Channel
        {
            public List<int> ChannelIds { get; set; }
            public int ContentId { get; set; }
        }

        private async Task<List<TableStyle>> GetStylesAsync(Channel channel)
        {
            var styles = new List<TableStyle>
            {
                new TableStyle()
                {
                    AttributeName = nameof(Channel.ImageUrl),
                    DisplayName = "栏目图片",
                    InputType = InputType.Image
                },
                new TableStyle()
                {
                    AttributeName = nameof(Channel.Content),
                    DisplayName = "栏目正文",
                    InputType = InputType.TextEditor
                }
            };
            var tableStyles = await _tableStyleRepository.GetChannelStylesAsync(channel);
            styles.AddRange(tableStyles);

            return styles;
        }
    }
}
