using System;
using System.Threading.Tasks;
using SSRAG.Utils;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using SSRAG.Datory;
using SSRAG.Configuration;
using System.Collections.Generic;
using System.IO;
using SSRAG.Datory.Utils;
using System.Linq;

namespace SSRAG.Datory
{
    public partial class DistributedCache
    {
        private readonly List<string> _keys = [];
        public List<string> GetKeys() => _keys.ToList();

        public void AddKey(string key)
        {
            if (!_keys.Contains(key)) _keys.Add(key);
        }

        public void RemoveKey(string key)
        {
            if (_keys.Contains(key)) _keys.Remove(key);
        }

        public void ClearKeys() => _keys.Clear();

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
        {
            // 1. 优先从本地内存缓存获取
            if (_memoryCache.TryGetValue(key, out T value))
            {
                return value;
            }

            // 2. 获取或创建锁，防止缓存击穿
            var lockKey = $"lock:{key}";
            var semaphore = _locks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();
            try
            {
                // 3. 再次检查本地缓存（可能在等待期间已被其他线程填充）
                if (_memoryCache.TryGetValue(key, out value))
                {
                    return value;
                }

                // 4. 从Redis获取

                var redisValue = await StringGetAsync(key);
                if (!string.IsNullOrEmpty(redisValue))
                {
                    value = TranslateUtils.JsonDeserialize<T>(redisValue);
                    _memoryCache.Set(key, value, TimeSpan.FromMinutes(Constants.DefaultMemoryExpireMinutes));
                    return value;
                }

                // 5. 如果Redis中也没有，则调用工厂方法生成数据
                value = await factory();
                if (value == null) return default;

                // 6. 同时写入本地缓存和Redis
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(Constants.DefaultMemoryExpireMinutes));
                await StringSetAsync(key, TranslateUtils.JsonSerialize(value));

                return value;
            }
            finally
            {
                semaphore.Release();
                _locks.TryRemove(lockKey, out _);
            }
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            await KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (_memoryCache.TryGetValue(key, out _))
            {
                return true;
            }

            return await KeyExistsAsync(key);
        }

        public async Task<string> GetStringAsync(string key)
        {
            // 1. 优先从本地内存缓存获取
            if (_memoryCache.TryGetValue(key, out string value))
            {
                return value;
            }

            // 2. 获取或创建锁，防止缓存击穿
            var lockKey = $"lock:{key}";
            var semaphore = _locks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();
            try
            {
                // 3. 再次检查本地缓存（可能在等待期间已被其他线程填充）
                if (_memoryCache.TryGetValue(key, out value))
                {
                    return value;
                }

                // 4. 从Redis获取
                var redisValue = await StringGetAsync(key);
                if (!string.IsNullOrEmpty(redisValue))
                {
                    value = redisValue;
                    _memoryCache.Set(key, value, TimeSpan.FromMinutes(Constants.DefaultMemoryExpireMinutes));
                    return value;
                }

                return string.Empty;
            }
            finally
            {
                semaphore.Release();
                _locks.TryRemove(lockKey, out _);
            }
        }

        public async Task<bool> SetStringAsync(string key, string value, int minutes = 0)
        {
            if (minutes > 0)
            {
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(minutes));
                return await StringSetAsync(key, value, minutes);
            }
            else
            {
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(Constants.DefaultMemoryExpireMinutes));
                return await StringSetAsync(key, value);
            }
        }

        public async Task<int> GetIntAsync(string key)
        {
            var value = await GetStringAsync(key);
            if (string.IsNullOrEmpty(value)) return 0;
            return TranslateUtils.ToInt(value);
        }

        public async Task<bool> SetIntAsync(string key, int value, int minutes = 0)
        {
            return await SetStringAsync(key, value.ToString(), minutes);
        }

        public async Task<bool> GetBoolAsync(string key)
        {
            var value = await GetStringAsync(key);
            if (string.IsNullOrEmpty(value)) return false;
            return TranslateUtils.ToBool(value);
        }

        public async Task<bool> SetBoolAsync(string key, bool value, int minutes = 0)
        {
            return await SetStringAsync(key, value.ToString(), minutes);
        }

        public async Task<T> GetObjectAsync<T>(string key)
        {
            var value = await GetStringAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return TranslateUtils.JsonDeserialize<T>(value);
            }

            return default;
        }

        public async Task<bool> SetObjectAsync<T>(string key, T value, int minutes = 0)
        {
            return await SetStringAsync(key, TranslateUtils.JsonSerialize(value), minutes);
        }

        public async Task<string> GetByFilePathAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return string.Empty;

            var pathKey = $"{filePath.Replace(Path.DirectorySeparatorChar, ':').Replace(Path.AltDirectorySeparatorChar, ':')}:{Utilities.GetUnixTimestamp(File.GetLastWriteTime(filePath))}";
            var value = await GetStringAsync(pathKey);

            if (!string.IsNullOrEmpty(value)) return value;

            value = await FileUtils.ReadTextAsync(filePath);
            await SetStringAsync(pathKey, value);

            return value;
        }

        public async Task<(bool IsConnectionWorks, string ErrorMessage)> IsConnectionWorksAsync()
        {
            var isSuccess = false;
            var errorMessage = string.Empty;
            try
            {
                var db = GetConnection().GetDatabase();

                var cacheCommand = "PING";
                var result = await db.ExecuteAsync(cacheCommand);

                isSuccess = result != null && result.ToString() == "PONG";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return (isSuccess, errorMessage);
        }

        public async Task ClearAsync()
        {
            _memoryCache.Clear();
            foreach (var key in GetKeys())
            {
                await KeyDeleteAsync(key);
            }
            ClearKeys();
        }
    }
}