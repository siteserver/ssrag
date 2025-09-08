using System.Threading.Tasks;

namespace SSRAG.Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual async Task RemoveCacheAsync(params string[] cacheKeys)
        {
            foreach (var cacheKey in cacheKeys)
            {
                await Cache.RemoveAsync(cacheKey);
            }
        }
    }
}
