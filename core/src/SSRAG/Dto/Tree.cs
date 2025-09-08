using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Dto
{
    public class Tree
    {
        public string DirectoryPath { get; }

        public Tree(ISettingsManager settingsManager,  string directory)
        {
            DirectoryPath = PathUtils.Combine(settingsManager.ContentRootPath, directory);
        }

        public string TablesFilePath => PathUtils.Combine(DirectoryPath, "_tables.json");

        public string GetTableMetadataFilePath(string tableName)
        {
            return PathUtils.Combine(DirectoryPath, tableName, "_metadata.json");
        }

        public string GetTableContentFilePath(string tableName, string fileName)
        {
            return PathUtils.Combine(DirectoryPath, tableName, fileName);
        }
    }
}
