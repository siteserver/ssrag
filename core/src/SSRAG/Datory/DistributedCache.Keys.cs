using System;
using SSRAG.Utils;

namespace SSRAG.Datory
{
    public partial class DistributedCache
    {
        public string GetClassKey(Type type, params string[] values)
        {
            if (values == null || values.Length <= 0) return type.FullName;
            return $"{type.FullName}:{ListUtils.ToString(values, ":")}";
        }

        public string GetEntityKey(string tableName)
        {
            return $"{tableName}:entity";
        }

        public string GetEntityKey(string tableName, int id)
        {
            return $"{tableName}:entity:{id}";
        }

        public string GetEntityKey(string tableName, string type, string identity)
        {
            return $"{tableName}:entity:{type}:{identity}";
        }

        public string GetListKey(string tableName)
        {
            return $"{tableName}:list";
        }

        public string GetListKey(string tableName, int siteId)
        {
            return $"{tableName}:list:{siteId}";
        }

        public string GetListKey(string tableName, string type)
        {
            return $"{tableName}:list:{type}";
        }

        public string GetListKey(string tableName, string type, params string[] identities)
        {
            return $"{tableName}:list:{type}:{ListUtils.ToString(identities, ":")}";
        }
    }
}