using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SqlKata;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Core.Repositories
{
    public partial class AdministratorRepository
    {
        private string GetCacheKeyByUserId(int userId)
        {
            return _settingsManager.Cache.GetEntityKey(TableName, "userId", userId.ToString());
        }

        private string GetCacheKeyByUuid(string uuid)
        {
            return _settingsManager.Cache.GetEntityKey(TableName, "uuid", uuid);
        }

        private string GetCacheKeyByUserName(string userName)
        {
            return _settingsManager.Cache.GetEntityKey(TableName, "userName", userName);
        }

        private string GetCacheKeyByMobile(string mobile)
        {
            return _settingsManager.Cache.GetEntityKey(TableName, "mobile", mobile);
        }

        private string GetCacheKeyByEmail(string email)
        {
            return _settingsManager.Cache.GetEntityKey(TableName, "email", email);
        }

        private List<string> GetCacheKeys(Administrator admin)
        {
            if (admin == null) return new List<string>();

            var keys = new List<string>
            {
                GetCacheKeyByUserId(admin.Id),
                GetCacheKeyByUuid(admin.Uuid),
                GetCacheKeyByUserName(admin.UserName)
            };

            if (!string.IsNullOrEmpty(admin.Mobile))
            {
                keys.Add(GetCacheKeyByMobile(admin.Mobile));
            }

            if (!string.IsNullOrEmpty(admin.Email))
            {
                keys.Add(GetCacheKeyByEmail(admin.Email));
            }

            return keys;
        }

        public async Task<Administrator> GetByAccountAsync(string account)
        {
            var admin = await GetByUserNameAsync(account);
            if (admin != null) return admin;
            if (StringUtils.IsMobile(account)) return await GetByMobileAsync(account);
            if (StringUtils.IsEmail(account)) return await GetByEmailAsync(account);

            return null;
        }

        private async Task<Administrator> GetAsync(Query query)
        {
            var admin = await _repository.GetAsync(query);

            if (admin != null && string.IsNullOrEmpty(admin.DisplayName))
            {
                admin.DisplayName = admin.UserName;
            }

            return admin;
        }

        public async Task<Administrator> GetByUserIdAsync(int userId)
        {
            if (userId <= 0) return null;

            return await GetAsync(Q
                .Where(nameof(Administrator.Id), userId)
                .CachingGet(GetCacheKeyByUserId(userId))
            );
        }

        public async Task<Administrator> GetByUuidAsync(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid)) return null;

            return await GetAsync(Q
                .Where(nameof(Administrator.Uuid), uuid)
                .CachingGet(GetCacheKeyByUuid(uuid))
            );
        }

        public async Task<Administrator> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            return await GetAsync(Q
                .Where(nameof(Administrator.UserName), userName)
                .CachingGet(GetCacheKeyByUserName(userName))
            );
        }

        public async Task<Administrator> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return null;

            return await GetAsync(Q
                .Where(nameof(Administrator.Mobile), mobile)
                .CachingGet(GetCacheKeyByMobile(mobile))
            );
        }

        public async Task<Administrator> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var cacheKey = GetCacheKeyByEmail(email);
            return await GetAsync(Q
                .Where(nameof(Administrator.Email), email)
                .CachingGet(cacheKey)
            );
        }

        public string GetUserUploadFileName(string filePath)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(filePath)}";
        }

        public async Task<string> GetDisplayAsync(int userId)
        {
            if (userId <= 0) return string.Empty;

            var admin = await GetByUserIdAsync(userId);
            return GetDisplay(admin);
        }

        public async Task<string> GetDisplayAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return string.Empty;

            var admin = await GetByUserNameAsync(userName);
            return GetDisplay(admin);
        }

        public string GetDisplay(Administrator admin)
        {
            if (admin == null) return string.Empty;

            return string.IsNullOrEmpty(admin.DisplayName) || admin.UserName == admin.DisplayName ? admin.UserName : $"{admin.DisplayName}({admin.UserName})";
        }
    }
}
