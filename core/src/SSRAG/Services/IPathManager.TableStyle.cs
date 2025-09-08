using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRAG.Services
{
    public partial interface IPathManager
    {
        Task<string> ExportStylesAsync(int siteId, string tableName, List<int> relatedIdentities);

        Task ImportStylesByDirectoryAsync(string tableName, List<int> relatedIdentities,
            string directoryPath);

        Task<string> ImportStylesByZipFileAsync(string tableName, List<int> relatedIdentities,
            string zipFilePath);
    }
}
