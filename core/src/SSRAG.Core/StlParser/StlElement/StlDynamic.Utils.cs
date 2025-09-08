﻿using System.Text;
using System.Threading.Tasks;
using SSRAG.Models;
using SSRAG.Parse;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.StlParser.StlElement
{
    public static partial class StlDynamic
    {
        public static Dynamic GetDynamicInfo(ISettingsManager settingsManager, string value, int page, User user, string pathAndQuery)
        {
            var dynamicInfo = TranslateUtils.JsonDeserialize<Dynamic>(settingsManager.Decrypt(value));
            if (dynamicInfo == null)
            {
                dynamicInfo = new Dynamic();
            }
            if (dynamicInfo.ChannelId == 0)
            {
                dynamicInfo.ChannelId = dynamicInfo.SiteId;
            }
            dynamicInfo.User = user;
            dynamicInfo.QueryString = PageUtils.GetQueryStringFilterSqlAndXss(pathAndQuery);
            dynamicInfo.QueryString.Remove("siteId");

            dynamicInfo.Page = page;

            return dynamicInfo;
        }

        public static async Task<string> GetScriptAsync(IParseManager parseManager, string dynamicApiUrl, Dynamic dynamicInfo)
        {
            if (string.IsNullOrEmpty(dynamicInfo.YesTemplate) &&
                string.IsNullOrEmpty(dynamicInfo.NoTemplate))
            {
                return string.Empty;
            }

            //运行解析以便为页面生成所需JS引用
            await parseManager.ParseInnerContentAsync(new StringBuilder(dynamicInfo.YesTemplate + dynamicInfo.NoTemplate));

            var values = parseManager.SettingsManager.Encrypt(TranslateUtils.JsonSerialize(dynamicInfo));
            var display = dynamicInfo.IsInline ? "inline-block" : "block";
            var elementId = dynamicInfo.ElementId;

            return $@"
<script id=""{elementId}"" type=""text/javascript"" language=""javascript"">
function stlDynamic{elementId}(page)
{{
    {dynamicInfo.OnBeforeSend}
    stlClient.post('{dynamicApiUrl}?' + StlClient.getQueryString(), {{
        value: '{values}',
        page: page
    }}, function (err, data, status) {{
        if (!err) {{
            if (data.value) {{
                {dynamicInfo.OnSuccess}
            }}
            $(""#{elementId}"").before(data.html);
        }} else {{
            {dynamicInfo.OnError}
        }}
        {dynamicInfo.OnComplete}
    }});
}}

function stlGetPage{elementId}(){{
    var page = 1;
    var queryString = document.location.search;
    if (queryString && queryString.length > 1) {{
        queryString = queryString.substring(1);
        var arr = queryString.split('&');
        for(var i=0; i < arr.length; i++) {{
            var item = arr[i];
            var arr2 = item.split('=');
            if (arr2 && arr2.length == 2) {{
                if (arr2[0] === 'page') {{
                    page = parseInt(arr2[1]);
                }}
            }}
        }}
    }}
    return page;
}}

stlDynamic{elementId}(stlGetPage{elementId}());

function stlRedirect{elementId}(page)
{{
    var queryString = document.location.search;
    var parameters = '';
    if (queryString && queryString.length > 1) {{
        queryString = queryString.substring(1);
        
        var arr = queryString.split('&');
        for(var i=0; i < arr.length; i++) {{
            var item = arr[i];
            var arr2 = item.split('=');
            if (arr2 && arr2.length == 2) {{
                if (arr2[0] !== 'page') {{
                    parameters += item + '&';
                }}
            }}
        }}
    }}
    parameters += 'page=' + page;
    location.href = location.protocol + '//' + location.host + location.pathname + '?' + parameters + location.hash;
}}
</script>
";
        }
    }
}
