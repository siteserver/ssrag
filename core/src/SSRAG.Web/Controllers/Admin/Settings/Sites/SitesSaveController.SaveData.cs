using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Core.Utils.Serialization;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesSaveController
    {
        [HttpPost, Route(RouteActionsData)]
        public async Task<ActionResult<BoolResult>> SaveData([FromBody] SaveRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(site.SiteType, request.TemplateDir);
            var siteContentDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.SiteContent);

            var caching = new Caching(_settingsManager);
            var exportObject = new ExportObject(_pathManager, _databaseManager, caching, site);
            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, request.IsSaveContents, request.IsSaveAllChannels, request.CheckedChannelIds);

            await SiteTemplateManager.ExportSiteToSiteTemplateAsync(_pathManager, _databaseManager, caching, site, request.TemplateDir);

            var siteTemplate = new SiteTemplate
            {
                SiteTemplateName = request.TemplateName,
                PicFileName = string.Empty,
                WebSiteUrl = request.WebSiteUrl,
                Description = request.Description
            };
            var xmlPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.Templates.FileMetadata);
            XmlUtils.SaveAsXml(siteTemplate, xmlPath);

            return new BoolResult
            {
                Value = true,
            };
        }
    }
}
