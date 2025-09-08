﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SSRAG.Datory;
using SSRAG.Datory.Utils;
using SSRAG.Core.Utils;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class DatabaseManager : IDatabaseManager
    {
        private readonly ISettingsManager _settingsManager;
        public IAccessTokenRepository AccessTokenRepository { get; }
        public IAdministratorRepository AdministratorRepository { get; }
        public IAdministratorsInRolesRepository AdministratorsInRolesRepository { get; }
        public ICeleryTaskRepository CeleryTaskRepository { get; }
        public IChannelRepository ChannelRepository { get; }
        public IChannelGroupRepository ChannelGroupRepository { get; }
        public IChatGroupRepository ChatGroupRepository { get; }
        public IChatMessageRepository ChatMessageRepository { get; }
        public IConfigRepository ConfigRepository { get; }
        public IContentCheckRepository ContentCheckRepository { get; }
        public IContentGroupRepository ContentGroupRepository { get; }
        public IContentRepository ContentRepository { get; }
        public IContentTagRepository ContentTagRepository { get; }
        public IDatasetRepository DatasetRepository { get; }
        public IDbCacheRepository DbCacheRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IDocumentRepository DocumentRepository { get; }
        public IErrorLogRepository ErrorLogRepository { get; }
        public IFlowEdgeRepository FlowEdgeRepository { get; }
        public IFlowNodeRepository FlowNodeRepository { get; }
        public IFlowVariableRepository FlowVariableRepository { get; }
        public IFormRepository FormRepository { get; }
        public IFormDataRepository FormDataRepository { get; }
        public ILogRepository LogRepository { get; }
        public IModelProviderRepository ModelProviderRepository { get; }
        public IModelRepository ModelRepository { get; }
        public IPermissionsInRolesRepository PermissionsInRolesRepository { get; }
        public IPluginConfigRepository PluginConfigRepository { get; }
        public IPromptRepository PromptRepository { get; }
        public IRelatedFieldItemRepository RelatedFieldItemRepository { get; }
        public IRelatedFieldRepository RelatedFieldRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IScheduledTaskRepository ScheduledTaskRepository { get; }
        public ISegmentRepository SegmentRepository { get; }
        public ISiteLogRepository SiteLogRepository { get; }
        public ISitePermissionsRepository SitePermissionsRepository { get; }
        public ISiteRepository SiteRepository { get; }
        public ISpecialRepository SpecialRepository { get; }
        public IStatRepository StatRepository { get; }
        public IStorageFileRepository StorageFileRepository { get; }
        public ITableStyleRepository TableStyleRepository { get; }
        public ITemplateLogRepository TemplateLogRepository { get; }
        public ITemplateRepository TemplateRepository { get; }
        public ITranslateRepository TranslateRepository { get; }
        public IUserGroupRepository UserGroupRepository { get; }
        public IUserMenuRepository UserMenuRepository { get; }
        public IUserRepository UserRepository { get; }
        public IUsersInGroupsRepository UsersInGroupsRepository { get; }

        public DatabaseManager(
            ISettingsManager settingsManager,
            IAccessTokenRepository accessTokenRepository,
            IAdministratorRepository administratorRepository,
            IAdministratorsInRolesRepository administratorsInRolesRepository,
            ICeleryTaskRepository celeryTaskRepository,
            IChannelRepository channelRepository,
            IChannelGroupRepository channelGroupRepository,
            IChatGroupRepository chatGroupRepository,
            IChatMessageRepository chatMessageRepository,
            IConfigRepository configRepository,
            IContentCheckRepository contentCheckRepository,
            IContentGroupRepository contentGroupRepository,
            IContentRepository contentRepository,
            IContentTagRepository contentTagRepository,
            IDatasetRepository datasetRepository,
            IDbCacheRepository dbCacheRepository,
            IDepartmentRepository departmentRepository,
            IDocumentRepository documentRepository,
            IErrorLogRepository errorLogRepository,
            IFlowEdgeRepository flowEdgeRepository,
            IFlowNodeRepository flowNodeRepository,
            IFlowVariableRepository flowVariableRepository,
            IFormRepository formRepository,
            IFormDataRepository formDataRepository,
            ILogRepository logRepository,
            IModelProviderRepository modelProviderRepository,
            IModelRepository modelRepository,
            IPermissionsInRolesRepository permissionsInRolesRepository,
            IPluginConfigRepository pluginConfigRepository,
            IPromptRepository promptRepository,
            IRelatedFieldItemRepository relatedFieldItemRepository,
            IRelatedFieldRepository relatedFieldRepository,
            IRoleRepository roleRepository,
            IScheduledTaskRepository scheduledTaskRepository,
            ISegmentRepository segmentRepository,
            ISiteLogRepository siteLogRepository,
            ISitePermissionsRepository sitePermissionsRepository,
            ISiteRepository siteRepository,
            ISpecialRepository specialRepository,
            IStatRepository statRepository,
            IStorageFileRepository storageFileRepository,
            ITableStyleRepository tableStyleRepository,
            ITemplateLogRepository templateLogRepository,
            ITemplateRepository templateRepository,
            ITranslateRepository translateRepository,
            IUserGroupRepository userGroupRepository,
            IUserMenuRepository userMenuRepository,
            IUserRepository userRepository,
            IUsersInGroupsRepository usersInGroupsRepository
        )
        {
            _settingsManager = settingsManager;
            AccessTokenRepository = accessTokenRepository;
            AdministratorRepository = administratorRepository;
            AdministratorsInRolesRepository = administratorsInRolesRepository;
            CeleryTaskRepository = celeryTaskRepository;
            ChannelRepository = channelRepository;
            ChannelGroupRepository = channelGroupRepository;
            ChatGroupRepository = chatGroupRepository;
            ChatMessageRepository = chatMessageRepository;
            ConfigRepository = configRepository;
            ContentCheckRepository = contentCheckRepository;
            ContentGroupRepository = contentGroupRepository;
            ContentRepository = contentRepository;
            ContentTagRepository = contentTagRepository;
            DatasetRepository = datasetRepository;
            DbCacheRepository = dbCacheRepository;
            DepartmentRepository = departmentRepository;
            DocumentRepository = documentRepository;
            ErrorLogRepository = errorLogRepository;
            FlowEdgeRepository = flowEdgeRepository;
            FlowNodeRepository = flowNodeRepository;
            FlowVariableRepository = flowVariableRepository;
            FormRepository = formRepository;
            FormDataRepository = formDataRepository;
            LogRepository = logRepository;
            ModelProviderRepository = modelProviderRepository;
            ModelRepository = modelRepository;
            PermissionsInRolesRepository = permissionsInRolesRepository;
            PluginConfigRepository = pluginConfigRepository;
            PromptRepository = promptRepository;
            RelatedFieldItemRepository = relatedFieldItemRepository;
            RelatedFieldRepository = relatedFieldRepository;
            RoleRepository = roleRepository;
            ScheduledTaskRepository = scheduledTaskRepository;
            SegmentRepository = segmentRepository;
            SiteLogRepository = siteLogRepository;
            SitePermissionsRepository = sitePermissionsRepository;
            SiteRepository = siteRepository;
            SpecialRepository = specialRepository;
            StatRepository = statRepository;
            StorageFileRepository = storageFileRepository;
            TableStyleRepository = tableStyleRepository;
            TemplateLogRepository = templateLogRepository;
            TemplateRepository = templateRepository;
            TranslateRepository = translateRepository;
            UserGroupRepository = userGroupRepository;
            UserMenuRepository = userMenuRepository;
            UserRepository = userRepository;
            UsersInGroupsRepository = usersInGroupsRepository;
        }

        public List<IRepository> GetAllRepositories()
        {
            var list = new List<IRepository>
            {
                AccessTokenRepository,
                AdministratorRepository,
                AdministratorsInRolesRepository,
                CeleryTaskRepository,
                ChannelRepository,
                ChannelGroupRepository,
                ChatGroupRepository,
                ChatMessageRepository,
                ConfigRepository,
                ContentCheckRepository,
                ContentGroupRepository,
                ContentRepository,
                ContentTagRepository,
                DatasetRepository,
                DbCacheRepository,
                DepartmentRepository,
                DocumentRepository,
                ErrorLogRepository,
                FlowEdgeRepository,
                FlowNodeRepository,
                FlowVariableRepository,
                FormRepository,
                FormDataRepository,
                LogRepository,
                ModelProviderRepository,
                ModelRepository,
                PermissionsInRolesRepository,
                PluginConfigRepository,
                PromptRepository,
                RelatedFieldItemRepository,
                RelatedFieldRepository,
                RoleRepository,
                ScheduledTaskRepository,
                SegmentRepository,
                SiteLogRepository,
                SitePermissionsRepository,
                SiteRepository,
                SpecialRepository,
                StatRepository,
                StorageFileRepository,
                TableStyleRepository,
                TemplateLogRepository,
                TemplateRepository,
                TranslateRepository,
                UserGroupRepository,
                UserMenuRepository,
                UserRepository,
                UsersInGroupsRepository,
            };

            return list;
        }

        public Database GetDatabase(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _settingsManager.Database.ConnectionString;
            }

            return new Database(_settingsManager.Database.DatabaseType, connectionString);
        }

        private IDbConnection GetConnection(string connectionString = null)
        {
            var database = GetDatabase(connectionString);
            return database.GetConnection();
        }

        private IDbConnection GetConnection(DatabaseType databaseType, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _settingsManager.Database.ConnectionString;
            }
            var database = new Database(databaseType, connectionString);
            return database.GetConnection();
        }

        public async Task DeleteDbLogAsync()
        {
            if (_settingsManager.Database.DatabaseType == DatabaseType.MySql || _settingsManager.Database.DatabaseType == DatabaseType.OceanBase)
            {
                using var connection = _settingsManager.Database.GetConnection();
                await connection.ExecuteAsync("PURGE MASTER LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 3 DAY)");
            }
            else if (_settingsManager.Database.DatabaseType == DatabaseType.SqlServer)
            {
                var databaseName = await _settingsManager.Database.GetDatabaseNamesAsync();

                using var connection = _settingsManager.Database.GetConnection();
                var versions = await connection.QueryFirstAsync<string>("SELECT SERVERPROPERTY('productversion')");

                var version = 8;
                var arr = versions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 0)
                {
                    version = TranslateUtils.ToInt(arr[0], 8);
                }
                if (version < 10)
                {
                    await connection.ExecuteAsync($"BACKUP LOG [{databaseName}] WITH NO_LOG");
                }
                else
                {
                    await connection.ExecuteAsync($@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ");
                }
            }
        }

        public int GetIntResult(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _settingsManager.Database.ConnectionString;
            }

            int count;

            var database = new Database(_settingsManager.Database.DatabaseType, connectionString);
            using (var conn = database.GetConnection())
            {
                count = conn.ExecuteScalar<int>(sqlString);
                //conn.Open();
                //using (var rdr = ExecuteReader(conn, sqlString))
                //{
                //    if (rdr.Read())
                //    {
                //        count = GetInt(rdr, 0);
                //    }
                //    rdr.Close();
                //}
            }
            return count;
        }

        public int GetIntResult(string sqlString)
        {
            return GetIntResult(_settingsManager.Database.ConnectionString, sqlString);
        }

        public string GetString(string connectionString, string sqlString)
        {
            string value;

            using (var connection = GetConnection(connectionString))
            {
                value = connection.ExecuteScalar<string>(sqlString);
            }

            return value;
        }

        private string GetString(string sqlString)
        {
            string value;

            using (var connection = GetConnection())
            {
                value = connection.ExecuteScalar<string>(sqlString);
            }

            return value;
        }

        public IEnumerable<IDictionary<string, object>> GetRows(DatabaseType databaseType, string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _settingsManager.Database.ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;

            IEnumerable<IDictionary<string, object>> rows;

            using (var connection = GetConnection(databaseType, connectionString))
            {
                rows = connection.Query(sqlString).Cast<IDictionary<string, object>>();
            }

            return rows;
        }

        public int GetPageTotalCount(string sqlString)
        {
            var temp = StringUtils.ToLower(sqlString);
            var pos = temp.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            // Add new ORDER BY info if SortKeyField is specified
            //if (!string.IsNullOrEmpty(sortField) && addCustomSortInfo)
            //    SelectCommand += " ORDER BY " + SortField;

            return GetIntResult($"SELECT COUNT(*) FROM ({sqlString}) AS T0");
        }

        public string GetStlPageSqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex)
        {
            string retVal;

            var temp = StringUtils.ToLower(sqlString);
            var pos = temp.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);
            if (pos > -1)
            {
                orderString = sqlString.Substring(pos);
                sqlString = sqlString.Substring(0, pos);
            }

            var recordsInLastPage = itemsPerPage;

            // Calculate the correspondent number of pages
            var lastPage = totalCount / itemsPerPage;
            var remainder = totalCount % itemsPerPage;
            if (remainder > 0)
                lastPage++;
            var pageCount = lastPage;

            if (remainder > 0)
                recordsInLastPage = remainder;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
                recsToRetrieve = recordsInLastPage;

            orderString = StringUtils.ToUpper(orderString);
            if (orderString.IndexOf(" ASC", StringComparison.OrdinalIgnoreCase) == -1 && orderString.IndexOf(" DESC", StringComparison.OrdinalIgnoreCase) == -1)
            {
                orderString += " ASC";
            }
            var orderStringReverse = orderString.Replace(" DESC", " DESC2");
            orderStringReverse = orderStringReverse.Replace(" ASC", " DESC");
            orderStringReverse = orderStringReverse.Replace(" DESC2", " ASC");

            if (_settingsManager.Database.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
            }
            else
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }

            return retVal;
        }

        public string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        private string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString, string orderByString, string joinString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = StringUtils.ReplaceStartsWith(whereString.Trim(), "AND", string.Empty);
                if (!StringUtils.StartsWithIgnoreCase(whereString, "WHERE "))
                {
                    whereString = "WHERE " + whereString;
                }
            }

            if (!string.IsNullOrEmpty(joinString))
            {
                whereString = joinString + " " + whereString;
            }

            return DbUtils.ToTopSqlString(_settingsManager.Database, tableName, columns, whereString, orderByString, totalNum);
        }

        public int GetCount(string tableName)
        {
            int count;

            using (var conn = _settingsManager.Database.GetConnection())
            {
                count = conn.ExecuteScalar<int>($"SELECT COUNT(*) FROM {Quote(tableName)}");
            }
            return count;

            //return GetIntResult();
        }

        public async Task<List<IDictionary<string, object>>> GetObjectsAsync(string tableName)
        {
            List<IDictionary<string, object>> objects;
            var sqlString = $"select * from {tableName}";

            await using (var connection = _settingsManager.Database.GetConnection())
            {
                objects = (from row in await connection.QueryAsync(sqlString)
                           select (IDictionary<string, object>)row).AsList();
            }

            return objects;
        }

        public async Task<List<IDictionary<string, object>>> GetPageObjectsAsync(string tableName, string identityColumnName, int offset, int limit)
        {
            List<IDictionary<string, object>> objects;
            var sqlString = GetPageSqlString(tableName, "*", string.Empty, $"ORDER BY {identityColumnName} ASC", offset, limit);

            await using (var connection = _settingsManager.Database.GetConnection())
            {
                objects = (from row in await connection.QueryAsync(sqlString)
                           select (IDictionary<string, object>)row).AsList();
            }

            return objects;
        }

        private decimal? _sqlServerVersion;

        private decimal SqlServerVersion
        {
            get
            {
                if (_settingsManager.Database.DatabaseType != DatabaseType.SqlServer)
                {
                    return 0;
                }

                if (_sqlServerVersion == null)
                {
                    try
                    {
                        _sqlServerVersion =
                            TranslateUtils.ToDecimal(
                                GetString("select left(cast(serverproperty('productversion') as varchar), 4)"));
                    }
                    catch
                    {
                        _sqlServerVersion = 0;
                    }
                }

                return _sqlServerVersion.Value;
            }
        }

        private bool IsSqlServer2012 => SqlServerVersion >= 11;

        public string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString, int offset, int limit)
        {
            var retVal = string.Empty;

            if (string.IsNullOrEmpty(orderSqlString))
            {
                orderSqlString = "ORDER BY Id DESC";
            }

            if (offset == 0 && limit == 0)
            {
                return $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString}";
            }

            if (_settingsManager.Database.DatabaseType == DatabaseType.MySql || _settingsManager.Database.DatabaseType == DatabaseType.OceanBase)
            {
                if (limit == 0)
                {
                    limit = int.MaxValue;
                }
                retVal = $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (_settingsManager.Database.DatabaseType == DatabaseType.SqlServer && IsSqlServer2012)
            {
                retVal = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }
            else if (_settingsManager.Database.DatabaseType == DatabaseType.SqlServer && !IsSqlServer2012)
            {
                if (offset == 0)
                {
                    retVal = $"SELECT TOP {limit} {columnNames} FROM {tableName} {whereSqlString} {orderSqlString}";
                }
                else
                {
                    var rowWhere = limit == 0
                        ? $@"WHERE [row_num] > {offset}"
                        : $@"WHERE [row_num] BETWEEN {offset + 1} AND {offset + limit}";

                    retVal = $@"SELECT * FROM (
    SELECT {columnNames}, ROW_NUMBER() OVER ({orderSqlString}) AS [row_num] FROM [{tableName}] {whereSqlString}
) as T {rowWhere}";
                }
            }
            else
            {
                retVal = limit == 0
                    ? $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }

            return retVal;
        }

        public string GetDatabaseNameFormConnectionString(string connectionString)
        {
            var name = GetValueFromConnectionString(connectionString, "Database");
            if (string.IsNullOrEmpty(name))
            {
                name = GetValueFromConnectionString(connectionString, "Initial Catalog");
            }
            return name;
        }

        private string GetValueFromConnectionString(string connectionString, string attribute)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(attribute))
            {
                var pairs = connectionString.Split(';');
                foreach (var pair in pairs)
                {
                    if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                    {
                        if (StringUtils.EqualsIgnoreCase(attribute, pair.Trim().Split('=')[0]))
                        {
                            retVal = pair.Trim().Split('=')[1];
                            break;
                        }
                    }
                }
            }
            return retVal;
        }
    }
}

