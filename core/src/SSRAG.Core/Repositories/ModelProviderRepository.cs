using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class ModelProviderRepository : IModelProviderRepository
    {
        private readonly Repository<ModelProvider> _repository;

        public ModelProviderRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ModelProvider>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}
