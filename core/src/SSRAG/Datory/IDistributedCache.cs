using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRAG.Datory
{
    public partial interface IDistributedCache
    {
        List<string> GetKeys();
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);

        Task RemoveAsync(string key);

        Task<bool> ExistsAsync(string key);

        Task<string> GetStringAsync(string key);

        Task<bool> SetStringAsync(string key, string value, int minutes = 0);

        Task<int> GetIntAsync(string key);

        Task<bool> SetIntAsync(string key, int value, int minutes = 0);

        Task<bool> GetBoolAsync(string key);

        Task<bool> SetBoolAsync(string key, bool value, int minutes = 0);

        Task<T> GetObjectAsync<T>(string key);

        Task<bool> SetObjectAsync<T>(string key, T value, int minutes = 0);

        Task<(bool IsConnectionWorks, string ErrorMessage)> IsConnectionWorksAsync();

        Task<string> GetByFilePathAsync(string filePath);

        Task ClearAsync();
    }
}