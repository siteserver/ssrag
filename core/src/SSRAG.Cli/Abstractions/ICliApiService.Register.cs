﻿using System.Threading.Tasks;

namespace SSRAG.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, string failureMessage)> RegisterAsync(string userName, string mobile, string email, string password);
    }
}
