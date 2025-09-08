﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Repositories
{
    public class StorageFileRepository : IStorageFileRepository
    {
        private readonly Repository<StorageFile> _repository;
        private readonly ISettingsManager _settingsManager;

        public StorageFileRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<StorageFile>(settingsManager.Database, settingsManager.Cache);
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string CacheKey => _settingsManager.Cache.GetClassKey(typeof(StorageFileRepository));

        public async Task<int> InsertAsync(StorageFile storageFile)
        {
            if (string.IsNullOrEmpty(storageFile.Key)) return 0;

            storageFile.FileType = FileUtils.GetFileType(PathUtils.GetExtension(storageFile.Key));
            storageFile.Id = await _repository.InsertAsync(storageFile, Q.CachingRemove(CacheKey));
            return storageFile.Id;
        }

        public async Task<bool> UpdateAsync(StorageFile storageFile)
        {
            if (string.IsNullOrEmpty(storageFile.Key)) return false;

            return await _repository.UpdateAsync(storageFile, Q.CachingRemove(CacheKey));
        }

        public async Task DeleteAsync(string key)
        {
            await _repository.DeleteAsync(Q
              .Where(nameof(StorageFile.Key), key)
              .CachingRemove(CacheKey)
            );
        }

        public async Task<List<StorageFile>> GetStorageFileListAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderBy(nameof(StorageFile.Key))
                .CachingGet(CacheKey)
            );
        }

        public async Task<StorageFile> GetStorageFileByKeyAsync(string key)
        {
            var storageFileList = await GetStorageFileListAsync();
            return storageFileList.FirstOrDefault(x => x.Key == key);
        }
    }
}
