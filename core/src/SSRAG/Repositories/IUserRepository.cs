﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Enums;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IUserRepository : IRepository
    {
        Task<(User user, string errorMessage)> InsertAsync(User user, string password, bool isChecked, string ipAddress);

        Task<int> InsertWithoutValidationAsync(User user, string password);

        Task<(bool success, string errorMessage)> UpdateAsync(User user);

        Task UpdateLastActivityDateAndCountOfLoginAsync(User user);

        Task UpdateLastActivityDateAsync(User user);

        Task<(bool success, string errorMessage)> ChangePasswordAsync(string userName, string password);

        Task<(bool success, string errorMessage)> IsPasswordCorrectAsync(string password);

        Task CheckAsync(IList<int> userIds);

        Task LockAsync(IList<int> userIds);

        Task UnLockAsync(IList<int> userIds);

        Task<bool> IsUserNameExistsAsync(string userName);

        Task<bool> IsEmailExistsAsync(string email);

        Task<bool> IsMobileExistsAsync(string mobile);

        Task<bool> IsOpenIdExistsAsync(string openId);

        Task<List<int>> GetUserIdsAsync(bool isChecked);

        bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, PasswordFormat passwordFormat,
            string passwordSalt);

        Task<(User user, string userName, string errorMessage)> ValidateAsync(string account, string password,
            bool isPasswordMd5);

        Task<(bool success, string errorMessage)> ValidateStateAsync(User user);

        Task<int> GetCountAsync();

        Task<int> GetCountAsync(bool? state, int groupId, int dayOfLastActivity, string keyword);

        Task<int> GetCountAsync(bool? state, bool? administrator, int departmentId, int groupId, int dayOfLastActivity, string keyword);

        Task<int> GetCountAsync(int departmentId, int groupId, string keyword);

        Task<List<User>> GetUsersAsync(bool? state, int groupId, int dayOfLastActivity, string keyword, string order, int offset, int limit);

        Task<List<User>> GetUsersAsync(bool? state, bool? manager, int departmentId, int groupId, int dayOfLastActivity, string keyword, string order, int offset, int limit);

        Task<List<User>> GetUsersAsync(int departmentId, int groupId, string keyword, int offset, int limit);

        Task<List<User>> GetUsersAsync(string keyword, bool isManagerOnly);

        Task<List<string>> GetUserNamesAsync(string keyword);

        Task<bool> IsExistsAsync(int id);

        Task<User> DeleteAsync(int userId);
    }
}
