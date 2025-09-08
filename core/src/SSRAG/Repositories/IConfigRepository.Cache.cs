using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface IConfigRepository
    {
        Task<Config> GetAsync();
    }
}
