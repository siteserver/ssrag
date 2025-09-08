using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Plugins;
using SSRAG.Services;

namespace SSRAG.Repositories
{
    public partial interface IContentRepository
    {
        Task TrashContentsAsync(Site site, Channel channel, List<int> contentIdList, string adminName);

        Task TrashContentsAsync(Site site, int channelId, string adminName);

        Task TrashContentAsync(Site site, Channel channel, int contentId, string adminName);

        Task DeletePreviewAsync(Site site, Channel channel);

        // 回收站 - 删除选中
        Task DeleteTrashAsync(Site site, int channelId, string tableName, List<int> contentIdList, IPluginManager pluginManager);

        // 回收站 - 删除全部
        Task DeleteTrashAsync(Site site, IPluginManager pluginManager);

        // 回收站 - 恢复全部
        Task RestoreTrashAsync(Site site, int restoreChannelId);

        // 回收站 - 恢复选中
        Task RestoreTrashAsync(Site site, int channelId, string tableName, List<int> contentIdList, int restoreChannelId);

        Task DeleteAsync(Site site, Channel channel, List<int> contentIdList, IPluginManager pluginManager);
    }
}
