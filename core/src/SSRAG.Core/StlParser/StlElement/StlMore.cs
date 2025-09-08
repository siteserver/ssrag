﻿using System.Threading.Tasks;
using SSRAG.Configuration;
using SSRAG.Core.StlParser.Attributes;
using SSRAG.Parse;
using SSRAG.Core.StlParser.Models;
using SSRAG.Core.StlParser.Utility;
using SSRAG.Models;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Enums;

namespace SSRAG.Core.StlParser.StlElement
{
    [StlElement(Title = "动态加载更多内容", Description = "通过 stl:more 标签在模板中动态加载更多内容")]
    public static class StlMore
    {
        public const string ElementName = "stl:more";

        [StlAttribute(Title = "触发函数")]
        public const string Trigger = nameof(Trigger);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var trigger = "more";

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Trigger))
                {
                    trigger = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
            }

            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            StlParserUtility.GetLoadingYesNo(contextInfo.InnerHtml, out var loading, out var yes, out var no);

            if (string.IsNullOrEmpty(loading))
            {
                var filePath = parseManager.PathManager.GetSiteFilesPath(Resources.Search.LoadingTemplatePath);
                loading = await parseManager.PathManager.GetContentByFilePathAsync(filePath);
            }
            if (string.IsNullOrEmpty(yes))
            {
                var filePath = parseManager.PathManager.GetSiteFilesPath(Resources.Search.YesTemplatePath);
                yes = await parseManager.PathManager.GetContentByFilePathAsync(filePath);
            }
            if (string.IsNullOrEmpty(no))
            {
                var filePath = parseManager.PathManager.GetSiteFilesPath(Resources.Search.NoTemplatePath);
                no = await parseManager.PathManager.GetContentByFilePathAsync(filePath);
            }

            pageInfo.AddPageHeadCodeIfNotExists(ParsePage.Const.Jquery);
            var elementId = StringUtils.GetElementId();

            var apiUrl = GetMoreApiUrl(pageInfo.Site, parseManager.PathManager);
            var apiParameters = GetMoreApiParameters(parseManager.SettingsManager, pageInfo.SiteId, pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.Template.TemplateType, yes);

            return @$"
<script id=""{elementId}"" type=""text/javascript"">
var {elementId}_page = 1;
function {trigger}(noMore, complete){{
  var parameters = {apiParameters};
  parameters['page'] = {elementId}_page++;
  $.ajax({{
    type: ""POST"",
    url: ""{apiUrl}"",
    contentType: ""application/json"",
    data: JSON.stringify(parameters),
    dataType: ""json"",
    success: function (result) {{
      $(""#{elementId}"").before(result.html);
      if (!result.value) {{
        noMore && noMore();
      }}
      complete && complete();
    }}
  }});
}};
</script>
";
        }

        public static string GetMoreApiUrl(Site site, IPathManager pathManager)
        {
            return pathManager.GetApiHostUrl(site, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsMore);
        }

        public static string GetMoreApiParameters(ISettingsManager settingsManager, int siteId, int pageChannelId, int pageContentId, TemplateType templateType, string template)
        {
            return TranslateUtils.JsonSerialize(new StlMoreRequest
            {
                SiteId = siteId,
                PageChannelId = pageChannelId,
                PageContentId = pageContentId,
                TemplateType = templateType,
                Template = settingsManager.Encrypt(template),
                Page = 1,
            });
        }
    }
}
