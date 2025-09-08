﻿using System.Threading.Tasks;

namespace SSRAG.Services
{
    public partial interface IDatabaseManager
    {
        Task<(bool success, string errorMessage)> InstallAsync(string userName, string password, string email,
            string mobile);

        Task CreateSiteServerTablesAsync();

        Task SyncDatabaseAsync();
    }
}
