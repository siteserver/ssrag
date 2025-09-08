﻿using Newtonsoft.Json;
using SSRAG.Services;

namespace SSRAG.Cli.Updater.Tables
{
    public partial class TableDbCache
    {
        private readonly IDatabaseManager _databaseManager;

        public TableDbCache(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("cacheKey")]
        public string CacheKey { get; set; }

        [JsonProperty("cacheValue")]
        public string CacheValue { get; set; }
    }
}
