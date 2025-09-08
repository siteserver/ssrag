using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using SSRAG.Configuration;

namespace SSRAG.Datory
{
    public partial class DistributedCache
    {
        private string GetRedisKey(string key)
        {
            if (!string.IsNullOrEmpty(_prefix) && !string.IsNullOrEmpty(key) && key.StartsWith(_prefix))
            {
                return key;
            }
            return _prefix + key;
        }

        public async Task<string> StringGetAsync(string key)
        {
            try
            {
                var db = GetConnection().GetDatabase();
                var value = await db.StringGetAsync(GetRedisKey(key));

                if (value.HasValue)
                {
                    AddKey(key);
                    return value.ToString();
                }

                RemoveKey(key);
                return string.Empty;
            }
            catch
            {
                RemoveKey(key);
                return string.Empty;
            }
        }

        public async Task<bool> StringSetAsync(string key, string value, int minutes = 0)
        {
            try
            {
                AddKey(key);
                var db = GetConnection().GetDatabase();
                if (minutes > 0)
                {
                    return await db.StringSetAsync(GetRedisKey(key), value, TimeSpan.FromMinutes(minutes));
                }
                else
                {
                    return await db.StringSetAsync(GetRedisKey(key), value, TimeSpan.FromMinutes(Constants.DefaultRedisExpireMinutes));
                }
            }
            catch
            {
                RemoveKey(key);
                return false;
            }
        }

        public async Task<bool> KeyDeleteAsync(string key)
        {
            try
            {
                RemoveKey(key);
                var db = GetConnection().GetDatabase();
                return await db.KeyDeleteAsync(GetRedisKey(key));
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            try
            {
                var db = GetConnection().GetDatabase();
                return await db.KeyExistsAsync(GetRedisKey(key));
            }
            catch
            {
                return false;
            }
        }
    }
}