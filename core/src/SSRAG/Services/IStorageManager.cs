using System;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Enums;

namespace SSRAG.Services
{
    public interface IStorageManager
    {
        Task<string> GetStorageUrlAsync(int siteId);

        Task<bool> IsStorageAsync(int siteId, SyncType syncType);

        Task<bool> IsAutoStorageAsync(int siteId, SyncType syncType);

        Task StorageAllAsync(int siteId);

        Task<(bool, string)> StorageAsync(int siteId, string filePath);

        void ClearStorageTasks(int siteId);

        SyncTaskSummary GetStorageTasks(int siteId);
    }
}
