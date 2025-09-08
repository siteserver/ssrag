using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Services
{
    public partial interface IPathManager
    {
        Task<string> GetSpecialUrlAsync(Site site, int specialId, bool isLocal);

        string GetSpecialDirectoryPath(Site site, string url);

        Task<string> GetSpecialUrlAsync(Site site, int specialId);

        string GetSpecialZipFilePath(string title, string directoryPath);

        string GetSpecialZipFileUrl(Site site, Special special);

        string GetSpecialSrcDirectoryPath(string directoryPath);

        Task<List<Template>> GetSpecialTemplateListAsync(Site site, int specialId);

        Task<Special> DeleteSpecialAsync(Site site, int specialId);
    }
}
