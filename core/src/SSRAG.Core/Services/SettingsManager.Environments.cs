using System;
using SSRAG.Datory;
using SSRAG.Datory.Utils;
using Microsoft.Extensions.Configuration;
using SSRAG.Core.Utils;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class SettingsManager
    {
        public const string TENANT_ID = "TENANT_ID";
        public const string SECURITY_KEY = "SECURITY_KEY";
        public const string DB_TYPE = "DB_TYPE";
        public const string DB_HOST = "DB_HOST";
        public const string DB_PORT = "DB_PORT";
        public const string DB_USER = "DB_USER";
        public const string DB_PASSWORD = "DB_PASSWORD";
        public const string DB_DATABASE = "DB_DATABASE";
        public const string DB_SCHEMA = "DB_SCHEMA";

        public const string REDIS_HOST = "REDIS_HOST";
        public const string REDIS_PORT = "REDIS_PORT";
        public const string REDIS_PASSWORD = "REDIS_PASSWORD";
        public const string REDIS_SSL = "REDIS_SSL";
        public const string REDIS_DB = "REDIS_DB";
        public const string REDIS_PREFIX = "REDIS_PREFIX";

        public const string MAX_SITES = "MAX_SITES";
        public const string DOTNET_RUNNING_IN_CONTAINER = "DOTNET_RUNNING_IN_CONTAINER";

        public static bool RunningInContainer => Environment.GetEnvironmentVariable(DOTNET_RUNNING_IN_CONTAINER) != null && TranslateUtils.ToBool(Environment.GetEnvironmentVariable(DOTNET_RUNNING_IN_CONTAINER));

        public void Reload()
        {
            TenantId = GetEnvironmentVariable(TENANT_ID);

            if (RunningInContainer)
            {
                SecurityKey = GetEnvironmentVariable(SECURITY_KEY);

                var envDbType = GetEnvironmentVariable(DB_TYPE);
                var envDbHost = GetEnvironmentVariable(DB_HOST);
                var envDbPort = GetEnvironmentVariable(DB_PORT);
                var envDbUser = GetEnvironmentVariable(DB_USER);
                var envDbPassword = GetEnvironmentVariable(DB_PASSWORD);
                var envDbDatabase = GetEnvironmentVariable(DB_DATABASE);
                var envDbSchema = GetEnvironmentVariable(DB_SCHEMA);

                var envRedisHost = GetEnvironmentVariable(REDIS_HOST);
                var envRedisPort = GetEnvironmentVariable(REDIS_PORT);
                var envRedisPassword = GetEnvironmentVariable(REDIS_PASSWORD);
                var envRedisSsl = GetEnvironmentVariable(REDIS_SSL);
                var envRedisDb = GetEnvironmentVariable(REDIS_DB);
                var envRedisPrefix = GetEnvironmentVariable(REDIS_PREFIX);

                var envMaxSites = TranslateUtils.ToInt(GetEnvironmentVariable(MAX_SITES));

                IsProtectData = false;
                IsSafeMode = false;
                DatabaseType = TranslateUtils.ToEnum(envDbType, DatabaseType.PostgreSql);
                var dbPort = TranslateUtils.ToInt(envDbPort);
                DatabaseConnectionString = DbUtils.GetConnectionString(DatabaseType, envDbHost, dbPort == 0, dbPort, envDbUser, envDbPassword, envDbDatabase, envDbSchema);
                var redisPort = TranslateUtils.ToInt(envRedisPort);
                RedisConnectionString = Utilities.GetRedisConnectionString(envRedisHost, redisPort == 0, redisPort, envRedisPassword, TranslateUtils.ToBool(envRedisSsl), envRedisDb, envRedisPrefix);
                MaxSites = envMaxSites;
            }
            else
            {
                IsProtectData = _config.GetValue(nameof(IsProtectData), false);
                IsSafeMode = _config.GetValue(nameof(IsSafeMode), false);
                SecurityKey = _config.GetValue<string>(nameof(SecurityKey));
                DatabaseType = TranslateUtils.ToEnum(
                    IsProtectData
                        ? Decrypt(_config.GetValue<string>("Database:Type"))
                        : _config.GetValue<string>("Database:Type"), DatabaseType.PostgreSql);
                DatabaseConnectionString = DatabaseType == DatabaseType.SQLite
                    ? $"Data Source={DbUtils.LocalDbHostVirtualPath};Version=3;"
                    : IsProtectData
                        ? Decrypt(_config.GetValue<string>("Database:ConnectionString"))
                        : _config.GetValue<string>("Database:ConnectionString");
                RedisConnectionString = IsProtectData
                    ? Decrypt(_config.GetValue<string>("Redis:ConnectionString"))
                    : _config.GetValue<string>("Redis:ConnectionString");
            }
            Cache = new DistributedCache(RedisConnectionString);
        }

        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}
