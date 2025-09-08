using System.Threading.Tasks;
using SSRAG.Core.Utils;
using SSRAG.Core.Utils.Serialization.Components;
using SSRAG.Dto;

namespace SSRAG.Core.Services
{
    public partial class FormManager
    {
        public async Task ImportFormAsync(int siteId, string directoryPath, bool overwrite)
        {
            var caching = new Caching(_settingsManager);
            var formIe = new FormIe(_pathManager, _databaseManager, caching, siteId, directoryPath);
            await formIe.ImportFormAsync(siteId, directoryPath, overwrite);
        }

        public async Task ExportFormAsync(int siteId, string directoryPath, int formId)
        {
            var caching = new Caching(_settingsManager);
            var formIe = new FormIe(_pathManager, _databaseManager, caching, siteId, directoryPath);
            await formIe.ExportFormAsync(siteId, directoryPath, formId);
        }
    }
}
