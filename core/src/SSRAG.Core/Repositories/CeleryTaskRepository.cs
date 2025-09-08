using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public class CeleryTaskRepository : ICeleryTaskRepository
    {
        private readonly Repository<CeleryTask> _repository;

        public CeleryTaskRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<CeleryTask>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<List<CeleryTask>> GetAllAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Select(nameof(CeleryTask.TaskId))
                .Where(nameof(CeleryTask.SiteId), siteId)
                .OrderBy(nameof(CeleryTask.TaskId))
            );
        }

        public async Task InsertAsync(CeleryTask task)
        {
            await _repository.InsertAsync(task);
        }

        public async Task UpdateAsync(CeleryTask task)
        {
            await _repository.UpdateAsync(task);
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(CeleryTask.SiteId), siteId));
        }
    }
}
