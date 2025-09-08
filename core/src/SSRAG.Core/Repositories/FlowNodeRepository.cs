using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class FlowNodeRepository : IFlowNodeRepository
    {
        private readonly Repository<FlowNode> _repository;

        public FlowNodeRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<FlowNode>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}
