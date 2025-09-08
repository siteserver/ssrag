using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IAccessTokenRepository
    {
        Task<AccessToken> GetByTokenAsync(string token);
    }
}
