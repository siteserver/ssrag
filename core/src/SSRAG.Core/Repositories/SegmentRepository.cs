using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class SegmentRepository : ISegmentRepository
    {
        private readonly Repository<Segment> _repository;

        public SegmentRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Segment>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}
