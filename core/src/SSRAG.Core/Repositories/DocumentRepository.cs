using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Core.Repositories
{
    public partial class DocumentRepository : IDocumentRepository
    {
        private readonly Repository<Document> _repository;

        public DocumentRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Document>(settingsManager.Database, settingsManager.Cache);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Document document)
        {
            return await _repository.InsertAsync(document);
        }

        public async Task<int> GetCountAsync(int siteId, int channelId)
        {
            return await _repository.CountAsync(Q.Where(nameof(Document.SiteId), siteId).Where(nameof(Document.ChannelId), channelId));
        }
    }
}
