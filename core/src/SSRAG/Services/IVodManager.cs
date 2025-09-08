using System.Threading.Tasks;
using SSRAG.Dto;

namespace SSRAG.Services
{
    public interface IVodManager
    {
        Task<VodSettings> GetVodSettingsAsync();

        Task<VodResult> UploadVodAsync(string filePath);
    }
}
