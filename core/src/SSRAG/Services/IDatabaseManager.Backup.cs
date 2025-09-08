using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Services
{
    public partial interface IDatabaseManager
    {
        Task<List<string>> BackupAsync(IConsoleUtils console, List<string> includes, List<string> excludes, int maxRows, int pageSize, Tree tree, string errorLogFilePath);
    }
}
