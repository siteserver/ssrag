using SSRAG.Services;
using System.Threading.Tasks;

namespace SSRAG.Dto
{
    public class Caching
    {
        private readonly ISettingsManager _settingsManager;
        public Caching(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        private static string GetProcessCacheKey(string uuid)
        {
            return $"{nameof(Process)}:{uuid}";
        }

        public async Task SetProcessAsync(string uuid, string message)
        {
            if (string.IsNullOrEmpty(uuid)) return;

            var cacheKey = GetProcessCacheKey(uuid);
            var process = await _settingsManager.Cache.GetObjectAsync<Process>(cacheKey);
            if (process == null)
            {
                process = new Process
                {
                    Total = 100,
                    Current = 0,
                    Message = message
                };
            }
            else
            {
                process.Total++;
                process.Current++;
                process.Message = message;
            }

            await _settingsManager.Cache.SetObjectAsync(cacheKey, process, 60);

        }

        public async Task<Process> GetProcessAsync(string uuid)
        {
            var cacheKey = GetProcessCacheKey(uuid);
            var process = await _settingsManager.Cache.GetObjectAsync<Process>(cacheKey) ?? new Process
            {
                Total = 100,
                Current = 0,
                Message = string.Empty
            };

            return process;
        }
    }
}
