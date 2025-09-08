using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Repositories
{
    public partial interface ISiteRepository
    {
        Task<Site> GetAsync(int siteId);
    }
}