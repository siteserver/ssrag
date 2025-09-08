using System.Collections.Generic;
using SSRAG.Configuration;

namespace SSRAG.Services
{
    public partial interface ISettingsManager
    {
        List<Permission> GetPermissions();
        List<Menu> GetMenus();
        List<Table> GetTables();
        List<string> GetContentTableNames();
    }
}
