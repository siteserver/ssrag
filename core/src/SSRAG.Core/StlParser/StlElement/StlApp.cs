using System.Collections.Specialized;
using System.Threading.Tasks;
using SSRAG.Configuration;
using SSRAG.Core.StlParser.Attributes;
using SSRAG.Datory;
using SSRAG.Models;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.StlParser.StlElement
{
    [StlElement(Title = "应用", Description = "通过 stl:app 标签在模板中显示应用")]
    public static class StlApp
    {
        public const string ElementName = "stl:app";

        [StlAttribute(Title = "应用名称")]
        private const string Name = nameof(Name);

        [StlAttribute(Title = "应用ID")]
        private const string Id = nameof(Id);

        [StlAttribute(Title = "是否点击图标打开应用")]
        private const string IsIcon = nameof(IsIcon);

        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        [StlAttribute(Title = "显示类型")]
        private const string DisplayType = nameof(DisplayType);

        public enum DisplayTypeEnum
        {
            Home,
            Chat,
            Copilot,
        }


        internal static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var appName = string.Empty;
            var appId = string.Empty;
            var isIcon = false;
            var height = "600px";
            var displayType = DisplayTypeEnum.Copilot;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Name))
                {
                    appName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Id))
                {
                    appId = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsIcon))
                {
                    isIcon = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, DisplayType))
                {
                    displayType = TranslateUtils.ToEnum(value, DisplayTypeEnum.Copilot);
                }
                else
                {
                    attributes[name] = value;
                }
            }

            var siteRepository = parseManager.DatabaseManager.SiteRepository;

            var site = !string.IsNullOrEmpty(appName) ? await siteRepository.GetSiteBySiteNameAsync(appName) : await siteRepository.GetAsync(TranslateUtils.ToInt(appId));

            if (site == null) return string.Empty;

            return Parse(parseManager, site, isIcon, height, displayType);
        }

        private static string Parse(IParseManager parseManager, Site site, bool isIcon, string height, DisplayTypeEnum displayType)
        {
            var elementId = StringUtils.GetElementId();
            var chatUrl = parseManager.PathManager.GetApiHostUrl(site, $"/open/{displayType.GetValue().ToLower()}/?id={site.Uuid}");
            var assetsUrl = parseManager.PathManager.GetApiHostUrl(site, "sitefiles/assets");
            var libUrl = parseManager.PathManager.GetApiHostUrl(site, "sitefiles/assets/lib/iframe-resizer-4.3.6/iframeResizer.min.js");

            if (isIcon)
            {
                return $@"
<script
  id=""chat-iframe""
  data-default-open=""false""
  data-drag=""false""
  defer
  data-agent-src=""{chatUrl}""
  data-open-icon=""{assetsUrl}/images/apps/chat-icon1.png""
  data-close-icon=""{assetsUrl}/images/apps/chat-icon1.png""
  src=""{assetsUrl}/js/apps/chat-iframe.js""
></script>
";
            }

            var heightStyle = !string.IsNullOrEmpty(height) ? $"height: {height}" : string.Empty;
            var frameResize = string.Empty;
            if (!string.IsNullOrEmpty(height))
            {
                heightStyle = $"height: {StringUtils.AddUnitIfNotExists(height)}";
            }
            else
            {
                frameResize = $@"
<script type=""text/javascript"" src=""{libUrl}""></script>
<script type=""text/javascript"">iFrameResize({{log: false}}, '#{elementId}')</script>
";
            }

            return $@"
<iframe id=""{elementId}"" frameborder=""0"" scrolling=""no"" src=""{chatUrl}&isIframe=true"" style=""width: 1px;min-width: 100%;min-height: 200px;{heightStyle}""></iframe>
{frameResize}
";
        }

    }
}
