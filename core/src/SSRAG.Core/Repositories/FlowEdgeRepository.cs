using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class FlowEdgeRepository : IFlowEdgeRepository
    {
        private readonly Repository<FlowEdge> _repository;

        public FlowEdgeRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<FlowEdge>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}