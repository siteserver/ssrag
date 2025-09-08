using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Utils;
using SSRAG.Core.Utils;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Datory;

namespace SSRAG.Web.Controllers.Admin.Apps
{
    public partial class AppsController
    {
        [HttpPost, Route(RouteDeleteCore)]
        public async Task<ActionResult<BoolResult>> DeleteCore([FromBody] DeleteCoreRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafeMode);
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (!StringUtils.EqualsIgnoreCase(site.SiteDir, StringUtils.Trim(request.SiteDir)))
            {
                return this.Error("删除失败，请输入正确的文件夹名称");
            }

            await _pathManager.DeleteSiteFilesAsync(site);
            await _authManager.AddAdminLogAsync($"删除{site.SiteType.GetValue()}", $"名称:{site.SiteName}");

            var list = await _channelRepository.GetChannelIdsAsync(request.SiteId);
            await _tableStyleRepository.DeleteAllAsync(site.TableName, list);
            await _channelGroupRepository.DeleteAllAsync(request.SiteId);
            await _contentGroupRepository.DeleteAllAsync(request.SiteId);
            await _contentTagRepository.DeleteAllAsync(request.SiteId);
            await _contentCheckRepository.DeleteAllAsync(request.SiteId);
            await _formRepository.DeleteAllAsync(request.SiteId);
            await _formDataRepository.DeleteAllAsync(request.SiteId);
            await _relatedFieldRepository.DeleteAllAsync(request.SiteId);
            await _relatedFieldItemRepository.DeleteAllAsync(request.SiteId);
            await _sitePermissionsRepository.DeleteAllAsync(request.SiteId);
            await _specialRepository.DeleteAllAsync(request.SiteId);
            await _statRepository.DeleteAllAsync(request.SiteId);
            await _templateLogRepository.DeleteAllAsync(request.SiteId);
            await _templateRepository.DeleteAllAsync(request.SiteId);
            await _translateRepository.DeleteAllAsync(request.SiteId);

            await _channelRepository.DeleteAllAsync(request.SiteId);
            await _siteRepository.DeleteAsync(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}