﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface IFormDataRepository : IRepository
    {
        Task<int> InsertAsync(Form form, FormData data);

        Task UpdateAsync(FormData data);

        Task<FormData> GetAsync(int id);

        Task<FormData> GetAsync(int dataId, int formId, List<TableStyle> styles);

        Task ReplyAsync(Form form, FormData data);

        Task DeleteByFormIdAsync(int formId);

        Task DeleteAsync(Form form, FormData data);

        Task DeleteAllAsync(int siteId);

        Task<int> GetCountAsync(int formId);

        Task<(int Total, List<FormData>)> GetListAsync(Form form, bool isRepliedOnly, DateTime? startDate, DateTime? endDate, string word, int page, int pageSize);

        Task<(int Total, List<FormData>)> GetListAsync(Form form, bool isRepliedOnly, int? channelId, int? contentId, string word, int page, int pageSize);

        Task<IList<FormData>> GetListAsync(Form form);

        string GetValue(TableStyle style, FormData data);
    }
}