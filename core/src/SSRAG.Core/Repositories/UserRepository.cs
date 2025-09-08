﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SSRAG.Datory;
using SqlKata;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Repositories
{
    public partial class UserRepository : IUserRepository
    {
        private readonly Repository<User> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUsersInGroupsRepository _usersInGroupsRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public UserRepository(
            ISettingsManager settingsManager,
            IConfigRepository configRepository,
            IUserGroupRepository userGroupRepository,
            IUsersInGroupsRepository usersInGroupsRepository,
            IDepartmentRepository departmentRepository
        )
        {
            _repository = new Repository<User>(settingsManager.Database, settingsManager.Cache);
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _userGroupRepository = userGroupRepository;
            _usersInGroupsRepository = usersInGroupsRepository;
            _departmentRepository = departmentRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private async Task<(bool success, string errorMessage)> InsertValidateAsync(string userName, string email, string mobile, string password, string ipAddress)
        {
            var config = await _configRepository.GetAsync();
            if (await IsIpAddressCachedAsync(ipAddress))
            {
                return (false, $"同一IP在{config.UserRegistrationMinMinutes}分钟内只能注册一次");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < config.UserPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.UserPasswordMinLength}");
            }
            if (!PasswordRestrictionUtils.IsValid(password, config.UserPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{config.UserPasswordRestriction.GetDisplayName()}");
            }
            if (string.IsNullOrEmpty(userName))
            {
                return (false, "用户名为空，请填写用户名");
            }
            if (!string.IsNullOrEmpty(userName) && await IsUserNameExistsAsync(userName))
            {
                return (false, "用户名已被注册，请更换用户名");
            }
            if (!IsUserNameCompliant(userName.Replace("@", string.Empty).Replace(".", string.Empty)))
            {
                return (false, "用户名包含不规则字符，请更换用户名");
            }

            if (!string.IsNullOrEmpty(email) && await IsEmailExistsAsync(email))
            {
                return (false, "电子邮件地址已被注册，请更换邮箱");
            }
            if (!string.IsNullOrEmpty(mobile) && await IsMobileExistsAsync(mobile))
            {
                return (false, "手机号码已被注册，请更换手机号码");
            }

            return (true, string.Empty);
        }

        private async Task<(User user, string errorMessage)> UpdateValidateAsync(User user)
        {
            if (user == null || user.Id <= 0)
            {
                return (null, "用户不存在");
            }

            var entity = await GetByUserIdAsync(user.Id);
            user.UserName = entity.UserName;
            user.Password = entity.Password;
            user.PasswordFormat = entity.PasswordFormat;
            user.PasswordSalt = entity.PasswordSalt;

            if (entity.Mobile != user.Mobile && !string.IsNullOrEmpty(user.Mobile) && await IsMobileExistsAsync(user.Mobile))
            {
                return (null, "手机号码已存在");
            }

            if (entity.Email != user.Email && !string.IsNullOrEmpty(user.Email) && await IsEmailExistsAsync(user.Email))
            {
                return (null, "邮箱地址已存在");
            }

            return (entity, string.Empty);
        }

        public async Task<(User user, string errorMessage)> InsertAsync(User user, string password, bool isChecked, string ipAddress)
        {
            // var config = await _configRepository.GetAsync();
            // if (!config.IsUserRegistrationAllowed)
            // {
            //     return (null, "对不起，系统已禁止新用户注册！");
            // }

            user.Checked = isChecked;
            if (StringUtils.IsMobile(user.UserName) && string.IsNullOrEmpty(user.Mobile))
            {
                user.Mobile = user.UserName;
            }

            var (success, errorMessage) = await InsertValidateAsync(user.UserName, user.Email, user.Mobile, password, ipAddress);
            if (!success)
            {
                return (null, errorMessage);
            }

            password = EncodePassword(password, PasswordFormat.SM4, out string passwordSalt);
            user.LastActivityDate = DateTime.Now;
            user.LastResetPasswordDate = DateTime.Now;

            user.Id = await InsertWithoutValidationAsync(user, password, PasswordFormat.SM4, passwordSalt);

            await CacheIpAddressAsync(ipAddress);

            return (user, string.Empty);
        }

        public async Task<int> InsertWithoutValidationAsync(User user, string password)
        {
            if (StringUtils.IsMobile(user.UserName) && string.IsNullOrEmpty(user.Mobile))
            {
                user.Mobile = user.UserName;
            }

            password = EncodePassword(password, PasswordFormat.SM4, out string passwordSalt);
            user.LastActivityDate = DateTime.Now;
            user.LastResetPasswordDate = DateTime.Now;

            return await InsertWithoutValidationAsync(user, password, PasswordFormat.SM4, passwordSalt);
        }

        private async Task<int> InsertWithoutValidationAsync(User user, string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            user.LastActivityDate = DateTime.Now;
            user.LastResetPasswordDate = DateTime.Now;

            user.Password = password;
            user.PasswordFormat = passwordFormat;
            user.PasswordSalt = passwordSalt;
            user.Set("ConfirmPassword", string.Empty);

            user.Id = await _repository.InsertAsync(user);
            await SyncDepartmentCountAsync(user.DepartmentId);

            return user.Id;
        }

        public async Task<(bool success, string errorMessage)> IsPasswordCorrectAsync(string password)
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(password))
            {
                return (false, "密码不能为空");
            }
            if (password.Length < config.UserPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.UserPasswordMinLength}");
            }
            if (!PasswordRestrictionUtils.IsValid(password, config.UserPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{config.UserPasswordRestriction.GetDisplayName()}");
            }
            return (true, string.Empty);
        }

        public async Task<(bool success, string errorMessage)> UpdateAsync(User user)
        {
            var (entity, errorMessage) = await UpdateValidateAsync(user);
            if (entity == null)
            {
                return (false, errorMessage);
            }

            user.Set("ConfirmPassword", string.Empty);
            var oldUser = await GetByUserIdAsync(user.Id);
            await _repository.UpdateAsync(user, Q.CachingRemove(GetCacheKeysToRemove(entity)));

            if (oldUser.DepartmentId != user.DepartmentId)
            {
                await SyncDepartmentCountAsync(oldUser.DepartmentId);
                await SyncDepartmentCountAsync(user.DepartmentId);
            }

            return (true, string.Empty);
        }

        private async Task UpdateLastActivityDateAndCountOfFailedLoginAsync(User user)
        {
            if (user == null) return;

            user.LastActivityDate = DateTime.Now;
            user.CountOfFailedLogin += 1;

            await _repository.UpdateAsync(Q
                .Set(nameof(User.LastActivityDate), user.LastActivityDate)
                .Set(nameof(User.CountOfFailedLogin), user.CountOfFailedLogin)
                .Where(nameof(User.Id), user.Id)
                .CachingRemove(GetCacheKeysToRemove(user))
            );
        }

        public async Task UpdateLastActivityDateAndCountOfLoginAsync(User user)
        {
            if (user == null) return;

            user.LastActivityDate = DateTime.Now;
            user.CountOfLogin += 1;
            user.CountOfFailedLogin = 0;

            await _repository.UpdateAsync(Q
                .Set(nameof(User.LastActivityDate), user.LastActivityDate)
                .Set(nameof(User.CountOfLogin), user.CountOfLogin)
                .Set(nameof(User.CountOfFailedLogin), user.CountOfFailedLogin)
                .Where(nameof(User.Id), user.Id)
                .CachingRemove(GetCacheKeysToRemove(user))
            );
        }

        public async Task UpdateLastActivityDateAsync(User user)
        {
            if (user == null) return;

            user.LastActivityDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(User.LastActivityDate), user.LastActivityDate)
                .Where(nameof(User.Id), user.Id)
                .CachingRemove(GetCacheKeysToRemove(user))
            );
        }

        private static string EncodePassword(string password, PasswordFormat passwordFormat, out string passwordSalt)
        {
            var retVal = string.Empty;
            passwordSalt = string.Empty;

            if (passwordFormat == PasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == PasswordFormat.Hashed)
            {
                passwordSalt = GenerateSalt();

                var src = Encoding.Unicode.GetBytes(password);
                var buffer2 = Convert.FromBase64String(passwordSalt);
                var dst = new byte[buffer2.Length + src.Length];
                byte[] inArray = null;
                Buffer.BlockCopy(buffer2, 0, dst, 0, buffer2.Length);
                Buffer.BlockCopy(src, 0, dst, buffer2.Length, src.Length);
                var algorithm = SHA1.Create(); // HashAlgorithm.Create("SHA1");
                if (algorithm != null) inArray = algorithm.ComputeHash(dst);

                if (inArray != null) retVal = Convert.ToBase64String(inArray);
            }
            else if (passwordFormat == PasswordFormat.Encrypted)
            {
                passwordSalt = GenerateSalt();
                retVal = DesEncryptor.EncryptStringBySecretKey(password, passwordSalt);
            }
            else if (passwordFormat == PasswordFormat.SM4)
            {
                passwordSalt = EncryptUtils.GenerateSecurityKey();
                retVal = EncryptUtils.Encrypt(password, passwordSalt);
            }

            return retVal;
        }

        private static string DecodePassword(string password, PasswordFormat passwordFormat, string passwordSalt)
        {
            var retVal = string.Empty;
            if (passwordFormat == PasswordFormat.Clear)
            {
                retVal = password;
            }
            else if (passwordFormat == PasswordFormat.Hashed)
            {
                throw new Exception("can not decode hashed password");
            }
            else if (passwordFormat == PasswordFormat.Encrypted)
            {
                retVal = DesEncryptor.DecryptStringBySecretKey(password, passwordSalt);
            }
            else if (passwordFormat == PasswordFormat.SM4)
            {
                retVal = EncryptUtils.Decrypt(password, passwordSalt);
            }

            return retVal;
        }

        private static string GenerateSalt()
        {
            var data = new byte[0x10];
            // new RNGCryptoServiceProvider().GetBytes(data);
            var rand = RandomNumberGenerator.Create();
            rand.GetBytes(data);

            return Convert.ToBase64String(data);
        }

        public async Task<(bool success, string errorMessage)> ChangePasswordAsync(string userName, string password)
        {
            var config = await _configRepository.GetAsync();
            if (password.Length < config.UserPasswordMinLength)
            {
                return (false, $"密码长度必须大于等于{config.UserPasswordMinLength}");
            }
            if (!PasswordRestrictionUtils.IsValid(password, config.UserPasswordRestriction))
            {
                return (false, $"密码不符合规则，请包含{config.UserPasswordRestriction.GetDisplayName()}");
            }

            password = EncodePassword(password, PasswordFormat.SM4, out string passwordSalt);
            await ChangePasswordAsync(userName, PasswordFormat.SM4, passwordSalt, password);
            return (true, string.Empty);
        }

        private async Task ChangePasswordAsync(string userName, PasswordFormat passwordFormat, string passwordSalt, string password)
        {
            var user = await GetByUserNameAsync(userName);
            if (user == null) return;

            user.LastResetPasswordDate = DateTime.Now;

            await _repository.UpdateAsync(Q
                .Set(nameof(User.Password), password)
                .Set(nameof(User.PasswordFormat), passwordFormat.GetValue())
                .Set(nameof(User.PasswordSalt), passwordSalt)
                .Set(nameof(User.LastResetPasswordDate), user.LastResetPasswordDate)
                .Where(nameof(User.Id), user.Id)
                .CachingRemove(GetCacheKeysToRemove(user))
            );
        }

        public async Task CheckAsync(IList<int> userIds)
        {
            var cacheKeys = new List<string>();
            foreach (var userId in userIds)
            {
                var user = await GetByUserIdAsync(userId);
                cacheKeys.AddRange(GetCacheKeysToRemove(user));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(User.Checked), true)
                .WhereIn(nameof(User.Id), userIds)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task LockAsync(IList<int> userIds)
        {
            var cacheKeys = new List<string>();
            foreach (var userId in userIds)
            {
                var user = await GetByUserIdAsync(userId);
                cacheKeys.AddRange(GetCacheKeysToRemove(user));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(User.Locked), true)
                .WhereIn(nameof(User.Id), userIds)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task UnLockAsync(IList<int> userIds)
        {
            var cacheKeys = new List<string>();
            foreach (var userId in userIds)
            {
                var user = await GetByUserIdAsync(userId);
                cacheKeys.AddRange(GetCacheKeysToRemove(user));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(User.Locked), false)
                .Set(nameof(User.CountOfFailedLogin), 0)
                .WhereIn(nameof(User.Id), userIds)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            return await _repository.ExistsAsync(Q.Where(nameof(User.UserName), userName));
        }

        private static bool IsUserNameCompliant(string userName)
        {
            if (userName.IndexOf("　", StringComparison.Ordinal) != -1 || userName.IndexOf(" ", StringComparison.Ordinal) != -1 || userName.IndexOf("'", StringComparison.Ordinal) != -1 || userName.IndexOf(":", StringComparison.Ordinal) != -1 || userName.IndexOf(".", StringComparison.Ordinal) != -1)
            {
                return false;
            }
            return DirectoryUtils.IsDirectoryNameCompliant(userName);
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;

            var exists = await IsUserNameExistsAsync(mobile);
            if (exists) return true;

            return await _repository.ExistsAsync(Q.Where(nameof(User.Mobile), mobile));
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var exists = await IsUserNameExistsAsync(email);
            if (exists) return true;

            return await _repository.ExistsAsync(Q.Where(nameof(User.Email), email));
        }

        public async Task<bool> IsOpenIdExistsAsync(string openId)
        {
            if (string.IsNullOrEmpty(openId)) return false;

            var exists = await IsUserNameExistsAsync(openId);
            if (exists) return true;

            return await _repository.ExistsAsync(Q.Where(nameof(User.OpenId), openId));
        }

        public async Task<List<int>> GetUserIdsAsync(bool isChecked)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(nameof(User.Id))
                .Where(nameof(User.Checked), isChecked)
                .OrderByDesc(nameof(User.Id))
            );
        }

        public bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, PasswordFormat passwordFormat, string passwordSalt)
        {
            var decodePassword = DecodePassword(dbPassword, passwordFormat, passwordSalt);
            if (isPasswordMd5)
            {
                return password == AuthUtils.Md5ByString(decodePassword);
            }
            return password == decodePassword;
        }

        public async Task<(User user, string userName, string errorMessage)> ValidateAsync(string account, string password, bool isPasswordMd5)
        {
            if (string.IsNullOrEmpty(account))
            {
                return (null, null, "账号不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return (null, null, "密码不能为空");
            }

            var user = await GetByAccountAsync(account);

            if (string.IsNullOrEmpty(user?.UserName))
            {
                return (null, null, "帐号或密码错误");
            }

            var (success, errorMessage) = await ValidateStateAsync(user);
            if (!success)
            {
                return (null, user.UserName, errorMessage);
            }

            var userEntity = await GetByUserIdAsync(user.Id);

            if (!CheckPassword(password, isPasswordMd5, userEntity.Password, userEntity.PasswordFormat, userEntity.PasswordSalt))
            {
                await UpdateLastActivityDateAndCountOfFailedLoginAsync(user);
                return (null, user.UserName, "帐号或密码错误");
            }

            return (user, user.UserName, string.Empty);
        }

        public async Task<(bool success, string errorMessage)> ValidateStateAsync(User user)
        {
            if (!user.Checked)
            {
                return (false, "此账号未审核，无法登录");
            }

            if (user.Locked)
            {
                return (false, "此账号被锁定，无法登录");
            }

            var config = await _configRepository.GetAsync();

            if (config.IsUserLockLogin)
            {
                if (user.CountOfFailedLogin > 0 && user.CountOfFailedLogin >= config.UserLockLoginCount)
                {
                    var lockType = TranslateUtils.ToEnum(config.UserLockLoginType, LockType.Hours);
                    if (lockType == LockType.Forever)
                    {
                        return (false, "此账号错误登录次数过多，已被永久锁定");
                    }
                    if (lockType == LockType.Hours && user.LastActivityDate.HasValue)
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - user.LastActivityDate.Value.Ticks);
                        var hours = Convert.ToInt32(config.UserLockLoginHours - ts.TotalHours);
                        if (hours > 0)
                        {
                            return (false, $"此账号错误登录次数过多，已被锁定，请等待{hours}小时后重试");
                        }
                    }
                }
            }

            return (true, null);
        }

        private async Task<Query> GetQueryAsync(bool? state, bool? manager, int departmentId, int groupId, int dayOfLastActivity, string keyword, string order)
        {
            var query = Q.NewQuery();

            if (state.HasValue)
            {
                query.Where(nameof(User.Checked), state.Value);
            }

            if (manager.HasValue)
            {
                query.Where(nameof(User.Manager), manager.Value);
            }

            if (dayOfLastActivity > 0)
            {
                var dateTime = DateTime.Now.AddDays(-dayOfLastActivity);
                query.Where(nameof(User.LastActivityDate), ">=", DateUtils.ToString(dateTime));
            }

            if (departmentId != -1)
            {
                if (departmentId > 0)
                {
                    var departmentIds = await _departmentRepository.GetDepartmentIdsAsync(departmentId, ScopeType.All);
                    query.WhereIn(nameof(User.DepartmentId), departmentIds);
                }
                else
                {
                    query
                      .Where(q => q
                          .Where(nameof(User.DepartmentId), 0)
                          .OrWhereNull(nameof(User.DepartmentId))
                      );
                }
            }

            if (groupId != -1)
            {
                if (groupId == _userGroupRepository.GroupIdOfManager)
                {
                    query.Where(nameof(User.Manager), true);
                }
                else if (groupId == _userGroupRepository.GroupIdOfDefault)
                {
                    var userIds = await _usersInGroupsRepository.GetUserIdsAsync();
                    query.WhereNotIn(nameof(User.Id), userIds);
                }
                else
                {
                    var userIds = await _usersInGroupsRepository.GetUserIdsAsync(groupId);
                    query.WhereIn(nameof(User.Id), userIds);
                }
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(User.UserName), like)
                    .OrWhereLike(nameof(User.Email), like)
                    .OrWhereLike(nameof(User.Mobile), like)
                    .OrWhereLike(nameof(User.DisplayName), like)
                );
            }

            if (!string.IsNullOrEmpty(order))
            {
                if (StringUtils.EqualsIgnoreCase(order, nameof(User.UserName)))
                {
                    query.OrderBy(nameof(User.UserName));
                }
                else
                {
                    query.OrderByDesc(order);
                }
            }
            else
            {
                query.OrderByDesc(nameof(User.Id));
            }

            return query;
        }

        public async Task<int> GetCountAsync()
        {
            return await _repository.CountAsync();
        }

        public async Task<int> GetCountAsync(bool? state, int groupId, int dayOfLastActivity, string keyword)
        {
            return await GetCountAsync(state, null, -1, groupId, dayOfLastActivity, keyword);
        }

        public async Task<int> GetCountAsync(bool? state, bool? manager, int departmentId, int groupId, int dayOfLastActivity, string keyword)
        {
            var query = await GetQueryAsync(state, manager, departmentId, groupId, dayOfLastActivity, keyword, string.Empty);
            return await _repository.CountAsync(query);
        }

        public async Task<int> GetCountAsync(int departmentId, int groupId, string keyword)
        {
            return await GetCountAsync(null, null, departmentId, groupId, 0, keyword);
        }

        public async Task<List<User>> GetUsersAsync(bool? state, int groupId, int dayOfLastActivity, string keyword, string order, int offset, int limit)
        {
            return await GetUsersAsync(state, null, -1, groupId, dayOfLastActivity, keyword, order, offset, limit);
        }

        public async Task<List<User>> GetUsersAsync(bool? state, bool? manager, int departmentId, int groupId, int dayOfLastActivity, string keyword, string order, int offset, int limit)
        {
            var query = await GetQueryAsync(state, manager, departmentId, groupId, dayOfLastActivity, keyword, order);
            query.Offset(offset).Limit(limit);

            return await _repository.GetAllAsync(query);
        }

        public async Task<List<User>> GetUsersAsync(int departmentId, int groupId, string keyword, int offset, int limit)
        {
            return await GetUsersAsync(null, null, departmentId, groupId, 0, keyword, null, offset, limit);
        }

        public async Task<List<User>> GetUsersAsync(string keyword, bool isManagerOnly)
        {
            return await GetUsersAsync(true, isManagerOnly ? true : null, -1, -1, 0, keyword, null, 0, 0);
        }

        public async Task<List<string>> GetUserNamesAsync(string keyword)
        {
            var query = Q.Select(nameof(User.UserName));

            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q => q
                    .WhereLike(nameof(User.UserName), like)
                    .OrWhereLike(nameof(User.Email), like)
                    .OrWhereLike(nameof(User.Mobile), like)
                    .OrWhereLike(nameof(User.DisplayName), like)
                );
            }

            return await _repository.GetAllAsync<string>(query);
        }

        public async Task<bool> IsExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<User> DeleteAsync(int userId)
        {
            var user = await GetByUserIdAsync(userId);

            await _repository.DeleteAsync(userId, Q.CachingRemove(GetCacheKeysToRemove(user)));
            await SyncDepartmentCountAsync(user.DepartmentId);
            await _usersInGroupsRepository.DeleteAllByUserIdAsync(userId);

            return user;
        }
    }
}

