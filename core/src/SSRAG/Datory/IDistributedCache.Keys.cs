using System;

namespace SSRAG.Datory
{
    public partial interface IDistributedCache
    {
        string GetClassKey(Type type, params string[] values);
        string GetEntityKey(string tableName);
        string GetEntityKey(string tableName, int id);
        string GetEntityKey(string tableName, string type, string identity);
        string GetListKey(string tableName);
        string GetListKey(string tableName, int siteId);
        string GetListKey(string tableName, string type);
        string GetListKey(string tableName, string type, params string[] identities);
    }
}
