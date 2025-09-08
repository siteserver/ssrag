using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IStorageFileRepository : IRepository
    {
        Task<int> InsertAsync(StorageFile storageFile);

        Task DeleteAsync(string key);

        Task<List<StorageFile>> GetStorageFileListAsync();

        Task<StorageFile> GetStorageFileByKeyAsync(string key);
    }
}