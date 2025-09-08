using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class PromptRepository : IPromptRepository
    {
        private readonly Repository<Prompt> _repository;

        public PromptRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Prompt>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}