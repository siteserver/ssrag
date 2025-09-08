using System;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using SSRAG.Datory.Utils;

namespace SSRAG.Datory
{
    public partial class DistributedCache : IDistributedCache
    {
        private readonly string _prefix;
        private readonly MemoryCache _memoryCache;
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public DistributedCache(string connectionString)
        {
            if (connectionString == null) return;

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            var (host, port, password, ssl, db, prefix) = Utilities.ParseRedisConnectionString(connectionString);
            _prefix = string.IsNullOrEmpty(prefix) ? "" : prefix + ":";
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var options = new ConfigurationOptions
                {
                    EndPoints = { $"{host}:{port}" },
                    ConnectTimeout = 5000,
                    AbortOnConnectFail = false,
                    Ssl = ssl,
                    Password = password,
                    DefaultDatabase = db,
                    ConnectRetry = 3
                };
                return ConnectionMultiplexer.Connect(options);
            });
        }

        private ConnectionMultiplexer GetConnection() => _lazyConnection.Value;
    }
}