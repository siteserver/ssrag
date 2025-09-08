using System;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Enums;

namespace SSRAG.Core.Services
{
    public partial class CloudManager
    {
        public Task<bool> IsStorageAsync()
        {
            return Task.FromResult(false);
        }

        public void ClearStorageTasks(int siteId)
        {
            throw new NotImplementedException();
        }

        public SyncTaskSummary GetStorageTasks(int siteId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStorageUrlAsync(int siteId)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> IsStorageAsync(int siteId, SyncType syncType)
        {
            return Task.FromResult(false);
        }

        public Task<bool> IsAutoStorageAsync(int siteId, SyncType syncType)
        {
            return Task.FromResult(false);
        }

        public Task StorageAllAsync(int siteId)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string)> StorageAsync(int siteId, string filePath)
        {
            return Task.FromResult((false, string.Empty));
        }
    }
}
