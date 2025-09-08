using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Core.Utils;
using SSRAG.Models;

namespace SSRAG.Core.Repositories
{
    public partial class AccessTokenRepository
    {
        private string GetCacheKeyByToken(string token)
        {
            return _settingsManager.Cache.GetEntityKey(TableName, "token", token);
        }

        public async Task<AccessToken> GetByTokenAsync(string token)
        {
            var cacheKey = GetCacheKeyByToken(token);

            return await _repository.GetAsync(Q
                .Where(nameof(AccessToken.Token),
                    _settingsManager.Encrypt(token))
                .CachingGet(cacheKey)
            );

            //var cacheKey = GetCacheKeyByToken(token);
            //return await
            //    _cache.GetOrCreateAsync(cacheKey, async entry =>
            //    {
            //        var accessToken = await _repository.GetAsync(Q
            //            .Where(nameof(AccessToken.Token), WebConfigUtils.EncryptStringBySecretKey(token)) 
            //        );

            //        return accessToken;
            //    });
        }
    }
}
