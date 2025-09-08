﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Services;

namespace SSRAG.Core.Services
{
    public partial class AuthManager : IAuthManager
    {
        private readonly IHttpContextAccessor _context;
        private readonly IAntiforgery _antiforgery;
        private readonly ClaimsPrincipal _principal;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly List<Permission> _permissions;

        public AuthManager(IHttpContextAccessor context, IAntiforgery antiforgery, ISettingsManager settingsManager, IDatabaseManager databaseManager)
        {
            _context = context;
            _antiforgery = antiforgery;
            _principal = context.HttpContext.User;
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _permissions = _settingsManager.GetPermissions();
        }

        private Administrator _admin;
        private User _user;

        public async Task InitAsync(User user)
        {
            if (user == null || user.Locked || !user.Checked)
            {
                _user = null;
                return;
            }

            var token = await _context.HttpContext.GetTokenAsync("access_token");
            var cachedToken = await _settingsManager.Cache.GetStringAsync(GetTokenCacheKey(_admin, user));
            if (!string.IsNullOrEmpty(cachedToken) && token != cachedToken)
            {
                _user = null;
                return;
            }

            _user = user;
        }

        public async Task<User> GetUserAsync()
        {
            if (!IsUser) return null;
            if (_user != null) return _user;

            _user = await _databaseManager.UserRepository.GetByUserNameAsync(UserName);

            await InitAsync(_user);

            return _user;
        }

        public async Task InitAsync(Administrator administrator)
        {
            if (administrator == null || administrator.Locked)
            {
                _admin = null;
                return;
            }

            var token = await _context.HttpContext.GetTokenAsync("access_token");
            var cachedToken = await _settingsManager.Cache.GetStringAsync(GetTokenCacheKey(_admin, _user));
            if (!string.IsNullOrEmpty(cachedToken) && token != cachedToken)
            {
                _admin = null;
                return;
            }

            _admin = administrator;
        }

        public async Task<Administrator> GetAdminAsync()
        {
            if (_admin != null) return _admin;

            if (IsAdmin)
            {
                _admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(AdminName);
                await InitAsync(_admin);
            }
            else if (IsUser)
            {
                var user = await GetUserAsync();
                if (user != null && !user.Locked && user.Checked)
                {
                    var groups = await _databaseManager.UsersInGroupsRepository.GetGroupsAsync(user);
                    foreach (var group in groups)
                    {
                        if (!string.IsNullOrEmpty(group.AdminName))
                        {
                            _admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(group.AdminName);
                        }
                        if (_admin != null) return _admin;
                    }
                }
            }
            else if (IsApi)
            {
                var tokenInfo = await _databaseManager.AccessTokenRepository.GetByTokenAsync(ApiToken);
                if (tokenInfo != null)
                {
                    if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                    {
                        var admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(tokenInfo.AdminName);
                        if (admin != null && !admin.Locked)
                        {
                            _admin = admin;
                        }
                    }
                }
            }

            return _admin;
        }

        public bool IsAdmin => _principal != null && _principal.IsInRole(Types.Roles.Administrator);

        public string AdminName => IsAdmin ? _principal.Claims.SingleOrDefault(c => c.Type == Types.Claims.AdminName)?.Value : string.Empty;

        public bool IsUser => _principal != null && _principal.IsInRole(Types.Roles.User);

        public string UserName => IsUser ? _principal.Claims.SingleOrDefault(c => c.Type == Types.Claims.UserName)?.Value : string.Empty;

        public bool IsApi => ApiToken != null;

        public string ApiToken
        {
            get
            {
                if (_context.HttpContext.Request.Query.TryGetValue("apiKey", out var queries))
                {
                    var token = queries.SingleOrDefault();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        return token;
                    }
                }
                if (_context.HttpContext.Request.Headers.TryGetValue("X-SS-API-KEY", out var headers))
                {
                    var token = headers.SingleOrDefault();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        return token;
                    }
                }

                return null;
            }
        }

        public string GetCSRFToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(_context.HttpContext);
            return tokens.RequestToken;
        }
    }
}