using System.Collections.Generic;

namespace SSRAG.Datory
{
    public interface IRepository
    {
        IDatabase Database { get; }

        string TableName { get; }

        List<TableColumn> TableColumns { get; }
    }
}
