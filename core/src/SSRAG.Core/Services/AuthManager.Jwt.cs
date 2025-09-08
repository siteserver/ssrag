using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Utils;
using System.Threading.Tasks;

namespace SSRAG.Core.Services
{
    public partial class AuthManager
    {
        private static ClaimsIdentity GetClaimsIdentity(Administrator administrator, User user, bool isPersistent)
        {
            var identity = new ClaimsIdentity();
            if (administrator != null)
            {
                identity.AddClaim(new Claim(Types.Claims.AdminName, administrator.UserName));
                identity.AddClaim(new Claim(Types.Claims.Role, Types.Roles.Administrator));
            }
            if (user != null)
            {
                identity.AddClaim(new Claim(Types.Claims.UserName, user.UserName));
                identity.AddClaim(new Claim(Types.Claims.Role, Types.Roles.User));
            }
            identity.AddClaim(new Claim(Types.Claims.IsPersistent, isPersistent.ToString()));
            return identity;
        }

        private static string GetTokenCacheKey(Administrator administrator, User user)
        {
            return $"admin:{administrator?.Id ?? 0}:user:{user?.Id ?? 0}:token";
        }

        public async Task<string> AuthenticateAsync(Administrator administrator, User user, bool isPersistent)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = EncryptUtils.GetSecurityKeyBytes(_settingsManager.SecurityKey);
            SecurityTokenDescriptor tokenDescriptor;
            var identity = GetClaimsIdentity(administrator, user, isPersistent);

            if (isPersistent)
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddDays(Constants.AccessTokenExpireDays),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
            }
            else
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            await _settingsManager.Cache.SetStringAsync(GetTokenCacheKey(administrator, user), tokenString);

            return tokenString;
        }

        // public async Task<string> RefreshAdministratorTokenAsync(string accessToken)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var principal = tokenHandler.ValidateToken(accessToken,
        //         new TokenValidationParameters
        //         {
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(EncryptUtils.GetSecurityKeyBytes(_settingsManager.SecurityKey)),
        //             ValidateIssuer = false,
        //             ValidateAudience = false
        //         }, out var validatedToken);
        //     var jwtToken = validatedToken as JwtSecurityToken;
        //     if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //     {
        //         throw new SecurityTokenException("Invalid token passed!");
        //     }

        //     var isPersistent = TranslateUtils.ToBool(_principal.Claims.SingleOrDefault(c => c.Type == Types.Claims.IsPersistent)?.Value);

        //     var administrator = await _databaseManager.AdministratorRepository.GetByUserNameAsync(principal.Identity.Name);
        //     return AuthenticateAdministrator(administrator, isPersistent);
        // }

        // private ClaimsIdentity GetUserIdentity(User user, bool isPersistent)
        // {
        //     return new ClaimsIdentity(new[]
        //     {
        //         new Claim(Types.Claims.UserId, user.Id.ToString()),
        //         new Claim(Types.Claims.UserName, user.UserName),
        //         new Claim(Types.Claims.Role, Types.Roles.User),
        //         new Claim(Types.Claims.IsPersistent, isPersistent.ToString())
        //     });
        // }

        // public string AuthenticateUser(User user, bool isPersistent)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var key = EncryptUtils.GetSecurityKeyBytes(_settingsManager.SecurityKey);
        //     SecurityTokenDescriptor tokenDescriptor;
        //     var identity = GetUserIdentity(user, isPersistent);

        //     if (isPersistent)
        //     {
        //         tokenDescriptor = new SecurityTokenDescriptor
        //         {
        //             Subject = identity,
        //             Expires = DateTime.Now.AddDays(Constants.AccessTokenExpireDays),
        //             SigningCredentials = new SigningCredentials(
        //                 new SymmetricSecurityKey(key),
        //                 SecurityAlgorithms.HmacSha256Signature)
        //         };
        //     }
        //     else
        //     {
        //         tokenDescriptor = new SecurityTokenDescriptor
        //         {
        //             Subject = identity,
        //             Expires = DateTime.Now.AddDays(1),
        //             SigningCredentials = new SigningCredentials(
        //                 new SymmetricSecurityKey(key),
        //                 SecurityAlgorithms.HmacSha256Signature)
        //         };
        //     }

        //     var token = tokenHandler.CreateToken(tokenDescriptor);

        //     var tokenString = tokenHandler.WriteToken(token);

        //     _cacheManager.AddOrUpdate(GetTokenCacheKey(user), tokenString);

        //     return tokenString;
        // }

        // public async Task<string> RefreshUserTokenAsync(string accessToken)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var principal = tokenHandler.ValidateToken(accessToken,
        //         new TokenValidationParameters
        //         {
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(EncryptUtils.GetSecurityKeyBytes(_settingsManager.SecurityKey)),
        //             ValidateIssuer = false,
        //             ValidateAudience = false
        //         }, out var validatedToken);
        //     var jwtToken = validatedToken as JwtSecurityToken;
        //     if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //     {
        //         throw new SecurityTokenException("Invalid token passed!");
        //     }

        //     var isPersistent = TranslateUtils.ToBool(_principal.Claims.SingleOrDefault(c => c.Type == Types.Claims.IsPersistent)?.Value);

        //     var user = await _databaseManager.UserRepository.GetByUserNameAsync(principal.Identity.Name);
        //     return AuthenticateUser(user, isPersistent);
        // }
    }
}
