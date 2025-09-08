using System.Collections.Generic;
using SSRAG.Configuration;

namespace SSRAG.Services
{
    public partial interface IAuthManager
    {
        bool IsMenuValid(Menu menu, IList<string> permissions);
    }
}
