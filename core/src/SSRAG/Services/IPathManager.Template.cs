using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Services
{
    public partial interface IPathManager
    {
        string GetTemplateFilePath(Site site, Template template);

        Task<string> GetTemplateContentAsync(Site site, Template template);

        Task WriteContentToTemplateFileAsync(Site site, Template template, string content, string adminName);

        Task<string> GetIncludeContentAsync(Site site, string file);

        Task WriteContentToIncludeFileAsync(Site site, string file, string content);

        Task<string> GetContentByFilePathAsync(string filePath);
    }
}
