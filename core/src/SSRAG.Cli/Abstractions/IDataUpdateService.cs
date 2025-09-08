using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Cli.Updater;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Cli.Abstractions
{
    public interface IDataUpdateService
    {
        void Load(Tree oldTree, Tree newTree);

        Task<Tuple<string, Table>> GetNewTableAsync(IConsoleUtils console, string oldTableName, Table oldTable,
            ConvertInfo converter);

        Task UpdateSplitContentsTableAsync(IConsoleUtils console, Dictionary<int, Table> splitSiteTableDict,
            List<int> siteIdList, string oldTableName, Table oldTable, ConvertInfo converter);

        Task<Tuple<string, Table>> UpdateTableAsync(IConsoleUtils console, string oldTableName, Table oldTable);
    }
}
