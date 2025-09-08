using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Utils;

namespace SSRAG.Cli.Updater
{
    public partial class TableContentConverter
    {
        public ConvertInfo GetSplitConverter()
        {
            return new ConvertInfo
            {
                NewColumns = GetNewColumns(null),
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        public ConvertInfo GetConverter(string oldTableName, List<TableColumn> oldColumns)
        {
            return new ConvertInfo
            {
                NewTableName = oldTableName,
                NewColumns = GetNewColumns(oldColumns),
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict,
                Process = Process
            };
        }

        private List<TableColumn> GetNewColumns(List<TableColumn> oldColumns)
        {
            var columns = _settingsManager.Database.GetTableColumns<Content>();

            if (oldColumns != null && oldColumns.Count > 0)
            {
                foreach (var column in oldColumns)
                {
                    if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(NodeId)))
                    {
                        column.AttributeName = nameof(SSRAG.Models.Content.ChannelId);
                    }
                    else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(PublishmentSystemId)))
                    {
                        column.AttributeName = nameof(SSRAG.Models.Content.SiteId);
                    }
                    else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(ContentGroupNameCollection)))
                    {
                        column.AttributeName = nameof(SSRAG.Models.Content.GroupNames);
                    }
                    else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(GroupNameCollection)))
                    {
                        column.AttributeName = nameof(SSRAG.Models.Content.GroupNames);
                    }
                    else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Tags)))
                    {
                        column.AttributeName = nameof(SSRAG.Models.Content.TagNames);
                    }

                    if (!columns.Exists(c => StringUtils.EqualsIgnoreCase(c.AttributeName, column.AttributeName)))
                    {
                        columns.Add(column);
                    }
                }
            }

            return columns;
        }

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(SSRAG.Models.Content.ChannelId), new[] {nameof(NodeId)}},
                {nameof(SSRAG.Models.Content.SiteId), new[] {nameof(PublishmentSystemId)}},
                {
                    nameof(SSRAG.Models.Content.GroupNames),
                    new[] {nameof(ContentGroupNameCollection), nameof(GroupNameCollection)}
                },
                {nameof(SSRAG.Models.Content.TagNames), new[] {nameof(Tags)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(Content), out var contentObj))
            {
                if (contentObj != null)
                {
                    var value = contentObj.ToString();
                    value = value.Replace("@upload", "@/upload");
                    row[nameof(SSRAG.Models.Content.Body)] = value;
                }
            }
            if (row.TryGetValue(nameof(SettingsXml), out contentObj))
            {
                if (contentObj != null)
                {
                    var value = contentObj.ToString();
                    value = value.Replace("@upload", "@/upload");
                    row["ExtendValues"] = value;
                }
            }
            if (row.TryGetValue(nameof(IsChecked), out contentObj))
            {
                if (contentObj != null)
                {
                    var value = TranslateUtils.ToBool(contentObj.ToString());
                    row[nameof(SSRAG.Models.Content.Checked)] = value;
                }
            }
            if (row.TryGetValue(nameof(IsTop), out contentObj))
            {
                if (contentObj != null)
                {
                    var value = TranslateUtils.ToBool(contentObj.ToString());
                    row[nameof(SSRAG.Models.Content.Top)] = value;
                }
            }
            if (row.TryGetValue(nameof(IsRecommend), out contentObj))
            {
                if (contentObj != null)
                {
                    var value = TranslateUtils.ToBool(contentObj.ToString());
                    row[nameof(SSRAG.Models.Content.Recommend)] = value;
                }
            }
            if (row.TryGetValue(nameof(IsHot), out contentObj))
            {
                if (contentObj != null)
                {
                    var value = TranslateUtils.ToBool(contentObj.ToString());
                    row[nameof(SSRAG.Models.Content.Hot)] = value;
                }
            }
            if (row.TryGetValue(nameof(IsColor), out contentObj))
            {
                if (contentObj != null)
                {
                    var value = TranslateUtils.ToBool(contentObj.ToString());
                    row[nameof(SSRAG.Models.Content.Color)] = value;
                }
            }

            return row;
        }
    }
}
