using System;
using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IErrorLogRepository
    {
        Task<int> AddErrorLogAsync(ErrorLog log);
        Task<int> AddErrorLogAsync(Exception ex, string summary = "");
        Task<int> AddErrorLogAsync(string pluginId, Exception ex, string summary = "");
    }
}
