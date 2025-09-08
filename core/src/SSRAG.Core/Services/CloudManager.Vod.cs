using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class CloudManager
    {
        public async Task<VodSettings> GetVodSettingsAsync()
        {
            return await Task.FromResult(new VodSettings());
        }

        public async Task<VodResult> UploadVodAsync(string filePath)
        {
            return await Task.FromResult(new VodResult());
        }
    }
}
