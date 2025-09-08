﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSRAG.Core.StlParser.Attributes;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.StlParser.StlElement
{
    [StlElement(Title = "当前位置", Description = "通过 stl:location 标签在模板中插入页面的当前位置")]
    public static class StlLocation
    {
        public const string ElementName = "stl:location";

        [StlAttribute(Title = "当前位置分隔符")]
        private const string Separator = nameof(Separator);

        [StlAttribute(Title = "打开窗口的目标")]
        private const string Target = nameof(Target);

        [StlAttribute(Title = "链接CSS样式")]
        private const string LinkClass = nameof(LinkClass);

        [StlAttribute(Title = "当前链接CSS样式")]
        private const string ActiveLinkClass = nameof(ActiveLinkClass);

        [StlAttribute(Title = "链接字数")]
        private const string WordNum = nameof(WordNum);

        [StlAttribute(Title = "是否包含当前栏目")]
        private const string IsContainSelf = nameof(IsContainSelf);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var separator = " - ";
            var target = string.Empty;
            var linkClass = string.Empty;
            var activeLinkClass = string.Empty;
            var wordNum = 0;
            var isContainSelf = true;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Separator))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Target))
                {
                    target = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LinkClass))
                {
                    linkClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ActiveLinkClass))
                {
                    activeLinkClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsContainSelf))
                {
                    isContainSelf = TranslateUtils.ToBool(value);
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, separator, target, linkClass, activeLinkClass, wordNum, isContainSelf, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string separator, string target, string linkClass, string activeLinkClass, int wordNum, bool isContainSelf, NameValueCollection attributes)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                separator = contextInfo.InnerHtml;
            }

            var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);

            var builder = new StringBuilder();

            var parentsCount = channel.ParentsCount;
            var nodePath = ListUtils.GetIntList(channel.ParentsPath);
            if (isContainSelf)
            {
                nodePath.Add(contextInfo.ChannelId);
            }
            foreach (var currentId in nodePath.Distinct())
            {
                var currentNodeInfo = await databaseManager.ChannelRepository.GetAsync(currentId);
                if (currentId == pageInfo.SiteId)
                {
                    var attrs = new NameValueCollection();
                    if (!string.IsNullOrEmpty(target))
                    {
                        attrs["target"] = target;
                    }
                    if (!string.IsNullOrEmpty(linkClass))
                    {
                        attrs["class"] = linkClass;
                    }
                    var url = await parseManager.PathManager.GetIndexPageUrlAsync(pageInfo.Site, pageInfo.IsLocal);
                    if (url.Equals(PageUtils.UnClickableUrl))
                    {
                        attrs["target"] = string.Empty;
                    }
                    attrs["href"] = url;
                    var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                    TranslateUtils.AddAttributesIfNotExists(attrs, attributes);

                    builder.Append($@"<a {TranslateUtils.ToAttributesString(attrs)}>{innerHtml}</a>");

                    if (parentsCount > 0)
                    {
                        builder.Append(separator);
                    }
                }
                else if (currentId == contextInfo.ChannelId)
                {
                    var attrs = new NameValueCollection();
                    if (!string.IsNullOrEmpty(target))
                    {
                        attrs["target"] = target;
                    }
                    if (!string.IsNullOrEmpty(linkClass))
                    {
                        attrs["class"] = linkClass;
                    }
                    if (!string.IsNullOrEmpty(activeLinkClass))
                    {
                        attrs["class"] = activeLinkClass;
                    }
                    var url = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, currentNodeInfo, pageInfo.IsLocal);
                    if (url.Equals(PageUtils.UnClickableUrl))
                    {
                        attrs["target"] = string.Empty;
                    }
                    attrs["href"] = url;
                    var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                    TranslateUtils.AddAttributesIfNotExists(attrs, attributes);

                    builder.Append($@"<a {TranslateUtils.ToAttributesString(attrs)}>{innerHtml}</a>");
                }
                else
                {
                    var attrs = new NameValueCollection();
                    if (!string.IsNullOrEmpty(target))
                    {
                        attrs["target"] = target;
                    }
                    if (!string.IsNullOrEmpty(linkClass))
                    {
                        attrs["class"] = linkClass;
                    }
                    var url = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, currentNodeInfo, pageInfo.IsLocal);
                    if (url.Equals(PageUtils.UnClickableUrl))
                    {
                        attrs["target"] = string.Empty;
                    }
                    attrs["href"] = url;
                    var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                    TranslateUtils.AddAttributesIfNotExists(attrs, attributes);

                    builder.Append($@"<a {TranslateUtils.ToAttributesString(attrs)}>{innerHtml}</a>");

                    if (parentsCount > 0)
                    {
                        builder.Append(separator);
                    }
                }
            }

            return builder.ToString();
        }
    }
}
