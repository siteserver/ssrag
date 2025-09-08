using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SqlKata;
using SSRAG.Core.Utils;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class ChatGroupRepository : IChatGroupRepository
    {
        private readonly Repository<ChatGroup> _repository;

        public ChatGroupRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ChatGroup>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}