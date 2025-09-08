﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IFormRepository : IRepository
    {
        Task<int> InsertAsync(Form form);

        Task<bool> UpdateAsync(Form form);

        Task DeleteAsync(int siteId, int formId);

        Task DeleteAllAsync(int siteId);

        Task UpdateTaxisToDownAsync(int siteId, int formId);

        Task UpdateTaxisToUpAsync(int siteId, int formId);

        Task<List<Form>> GetFormsAsync(int siteId);

        Task<string> GetImportTitleAsync(int siteId, string title);

        Task<Form> GetAsync(int siteId, int id);

        Task<Form> GetByTitleAsync(int siteId, string title);

        List<string> GetAllAttributeNames(List<TableStyle> styles);

        List<int> GetRelatedIdentities(int formId);

        Task<List<TableStyle>> GetTableStylesAsync(int formId);

        Task DeleteTableStyleAsync(int formId, string attributeName);

        Task CreateDefaultStylesAsync(Form form);

        List<ContentColumn> GetColumns(List<string> listAttributeNames, List<TableStyle> styles, bool isReply);

        int GetPageSize(Form form);
    }
}