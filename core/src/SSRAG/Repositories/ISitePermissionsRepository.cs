﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public interface ISitePermissionsRepository : IRepository
    {
        Task InsertAsync(SitePermissions permissions);

        Task DeleteAsync(string roleName);

        Task DeleteAllAsync(int siteId);

        Task<List<SitePermissions>> GetAllAsync(string roleName);

        Task<SitePermissions> GetAsync(string roleName, int siteId);

        Task<Dictionary<int, List<string>>> GetSitePermissionDictionaryAsync(IEnumerable<string> roles);

        Task<Dictionary<string, List<string>>> GetContentPermissionDictionaryAsync(IList<string> roles);
    }
}
