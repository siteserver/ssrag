using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SSRAG.Configuration;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Parse;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Utils
{
    public class InputParserManager
    {
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly IPathManager _pathManager;

        public InputParserManager(IPathManager pathManager, IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            _pathManager = pathManager;
            _relatedFieldItemRepository = relatedFieldItemRepository;
        }

        public string GetContentByTableStyle(string content, string separator, Site site, TableStyle style, string formatString, NameValueCollection attributes, string innerHtml, bool isStlEntity)
        {
            var parsedContent = content;

            var inputType = style.InputType;

            if (inputType == InputType.Date)
            {
                var dateTime = TranslateUtils.ToDateTime(content);
                if (dateTime != Constants.SqlMinValue)
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        formatString = DateUtils.FormatStringDateOnly;
                    }
                    parsedContent = DateUtils.Format(dateTime, formatString);
                }
                else
                {
                    parsedContent = string.Empty;
                }
            }
            else if (inputType == InputType.DateTime)
            {
                var dateTime = TranslateUtils.ToDateTime(content);
                if (dateTime != Constants.SqlMinValue)
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        formatString = DateUtils.FormatStringDateTime;
                    }
                    parsedContent = DateUtils.Format(dateTime, formatString);
                }
                else
                {
                    parsedContent = string.Empty;
                }
            }
            else if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)//选择类型
            {
                var selectedTexts = new List<string>();
                var selectedValues = ListUtils.GetStringList(content);
                var styleItems = style.Items;
                if (styleItems != null)
                {
                    foreach (var itemInfo in styleItems)
                    {
                        if (selectedValues.Contains(itemInfo.Value))
                        {
                            selectedTexts.Add(isStlEntity ? itemInfo.Value : itemInfo.Label);
                        }
                    }
                }

                parsedContent = separator == null ? ListUtils.ToString(selectedTexts) : ListUtils.ToString(selectedTexts, separator);
            }
            //else if (style.InputType == InputType.TextArea)
            //{
            //    parsedContent = StringUtils.ReplaceNewlineToBR(parsedContent);
            //}
            else if (inputType == InputType.TextEditor)
            {
                parsedContent = _pathManager.DecodeTextEditor(site, parsedContent, true);
            }
            else if (inputType == InputType.Image)
            {
                parsedContent = GetImageOrFlashHtml(site, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.Video)
            {
                parsedContent = GetVideoHtml(site, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.File)
            {
                parsedContent = GetFileHtmlWithoutCount(site, parsedContent, attributes, innerHtml, isStlEntity, false, false);
            }

            return parsedContent;
        }

        public async Task<string> GetContentByTableStyleAsync(Content content, string separator, Site site, TableStyle style, string formatString, int no, NameValueCollection attributes, string innerHtml, bool isStlEntity)
        {
            var obj = content.Get(style.AttributeName);
            if (obj == null) return string.Empty;

            var parsedContent = string.Empty;

            var inputType = style.InputType;

            if (inputType == InputType.Date)
            {
                var dateTime = TranslateUtils.ToDateTime(obj.ToString());
                if (dateTime != Constants.SqlMinValue)
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        formatString = DateUtils.FormatStringDateOnly;
                    }
                    parsedContent = DateUtils.Format(dateTime, formatString);
                }
            }
            else if (inputType == InputType.DateTime)
            {
                var dateTime = TranslateUtils.ToDateTime(obj.ToString());
                if (dateTime != Constants.SqlMinValue)
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        formatString = DateUtils.FormatStringDateTime;
                    }
                    parsedContent = DateUtils.Format(dateTime, formatString);
                }
            }
            else if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)//选择类型
            {
                var selectedTexts = new List<string>();
                var selectedValues = ListUtils.ToList(obj);
                var styleItems = style.Items;
                if (styleItems != null)
                {
                    foreach (var itemInfo in styleItems)
                    {
                        if (selectedValues.Contains(itemInfo.Value))
                        {
                            selectedTexts.Add(isStlEntity ? itemInfo.Value : itemInfo.Label);
                        }
                    }
                }

                parsedContent = separator == null ? ListUtils.ToString(selectedTexts) : ListUtils.ToString(selectedTexts, separator);
            }
            else if (inputType == InputType.TextEditor)
            {
                parsedContent = _pathManager.DecodeTextEditor(site, obj.ToString(), true);
            }
            else if (inputType == InputType.Image)
            {
                if (no <= 1)
                {
                    parsedContent = GetImageOrFlashHtml(site, obj.ToString(), attributes, isStlEntity);
                }
                else
                {
                    var extendName = ColumnsManager.GetExtendName(style.AttributeName, no - 1);
                    var extend = content.Get<string>(extendName);
                    parsedContent = GetImageOrFlashHtml(site, extend, attributes, isStlEntity);
                }
            }
            else if (inputType == InputType.Video)
            {
                if (no <= 1)
                {
                    parsedContent = GetVideoHtml(site, obj.ToString(), attributes, isStlEntity);
                }
                else
                {
                    var extendName = ColumnsManager.GetExtendName(style.AttributeName, no - 1);
                    var extend = content.Get<string>(extendName);
                    parsedContent = GetVideoHtml(site, extend, attributes, isStlEntity);
                }
            }
            else if (inputType == InputType.File)
            {
                if (no <= 1)
                {
                    parsedContent = GetFileHtmlWithoutCount(site, obj.ToString(), attributes, innerHtml, isStlEntity, false, false);
                }
                else
                {
                    var extendName = ColumnsManager.GetExtendName(style.AttributeName, no - 1);
                    var extend = content.Get<string>(extendName);
                    parsedContent = GetFileHtmlWithoutCount(site, extend, attributes, innerHtml, isStlEntity, false, false);
                }
            }
            else if (inputType == InputType.SelectCascading)
            {
                var texts = obj.ToString();
                var selectedTexts = new List<string>();
                if (!string.IsNullOrEmpty(texts))
                {
                    var itemIds = ListUtils.GetIntList(texts.Trim('[').Trim(']'));
                    foreach (var itemId in itemIds)
                    {
                        var value = await _relatedFieldItemRepository.GetValueAsync(site.Id, itemId);
                        if (!string.IsNullOrEmpty(value))
                        {
                            selectedTexts.Add(value);
                        }
                    }
                }
                parsedContent = separator == null ? ListUtils.ToString(selectedTexts) : ListUtils.ToString(selectedTexts, separator);
            }
            else
            {
                parsedContent = obj.ToString();
            }

            return parsedContent;
        }

        public string GetImageOrFlashHtml(Site site, string imageUrl, NameValueCollection attributes, bool isStlEntity)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = _pathManager.ParseSiteUrl(site, imageUrl, false);
                if (isStlEntity)
                {
                    retVal = imageUrl;
                }
                else
                {
                    if (!imageUrl.ToUpper().Trim().EndsWith(".SWF"))
                    {
                        var imageAttributes = new NameValueCollection();
                        TranslateUtils.AddAttributesIfNotExists(imageAttributes, attributes);
                        imageAttributes["src"] = imageUrl;

                        retVal = $@"<img {TranslateUtils.ToAttributesString(imageAttributes)}>";
                    }
                    else
                    {
                        var width = 100;
                        var height = 100;
                        if (attributes != null)
                        {
                            if (!string.IsNullOrEmpty(attributes["width"]))
                            {
                                width = TranslateUtils.ToInt(attributes["width"]);
                            }
                            if (!string.IsNullOrEmpty(attributes["height"]))
                            {
                                height = TranslateUtils.ToInt(attributes["height"]);
                            }
                        }
                        retVal = $@"
<object classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" codebase=""http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0"" width=""{width}"" height=""{height}"">
                <param name=""movie"" value=""{imageUrl}"">
                <param name=""quality"" value=""high"">
                <param name=""wmode"" value=""transparent"">
                <embed src=""{imageUrl}"" width=""{width}"" height=""{height}"" quality=""high"" pluginspage=""http://www.macromedia.com/go/getflashplayer"" type=""application/x-shockwave-flash"" wmode=""transparent""></embed></object>
";
                    }
                }
            }
            return retVal;
        }

        public string GetVideoHtml(Site site, string videoUrl, NameValueCollection attributes, bool isStlEntity)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(videoUrl))
            {
                videoUrl = _pathManager.ParseSiteUrl(site, videoUrl, false);
                if (isStlEntity)
                {
                    retVal = videoUrl;
                }
                else
                {
                    var url = _pathManager.GetSiteFilesUrl(site, Resources.BrPlayer.Swf);
                    retVal = $@"
<embed src=""{url}"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart={StringUtils.ToLower(true
                        .ToString())}&image={string.Empty}&file={videoUrl}"" width=""{450}"" height=""{350}""/>
";
                }
            }
            return retVal;
        }

        public string GetFileHtmlWithCount(Site site, int channelId, int contentId, string fileUrl, NameValueCollection attributes, string innerHtml, bool isStlEntity, bool isLower, bool isUpper)
        {
            if (site == null || string.IsNullOrEmpty(fileUrl)) return string.Empty;

            string retVal;
            if (isStlEntity)
            {
                retVal = _pathManager.GetDownloadApiUrl(site, channelId, contentId,
                    fileUrl);
            }
            else
            {
                var linkAttributes = new NameValueCollection();
                TranslateUtils.AddAttributesIfNotExists(linkAttributes, attributes);
                linkAttributes["href"] = _pathManager.GetDownloadApiUrl(site, channelId,
                    contentId, fileUrl);

                innerHtml = string.IsNullOrEmpty(innerHtml)
                    ? PageUtils.GetFileNameFromUrl(fileUrl)
                    : innerHtml;

                if (isLower)
                {
                    innerHtml = StringUtils.ToLower(innerHtml);
                }
                if (isUpper)
                {
                    innerHtml = StringUtils.ToUpper(innerHtml);
                }

                retVal = $@"<a {TranslateUtils.ToAttributesString(linkAttributes)}>{innerHtml}</a>";
            }

            return retVal;
        }

        public string GetFileHtmlWithoutCount(Site site, string fileUrl, NameValueCollection attributes, string innerHtml, bool isStlEntity, bool isLower, bool isUpper)
        {
            if (site == null || string.IsNullOrEmpty(fileUrl)) return string.Empty;

            string retVal;
            if (isStlEntity)
            {
                retVal = _pathManager.GetDownloadApiUrl(site, fileUrl);
            }
            else
            {
                var linkAttributes = new NameValueCollection();
                TranslateUtils.AddAttributesIfNotExists(linkAttributes, attributes);
                linkAttributes["href"] = _pathManager.GetDownloadApiUrl(site, fileUrl);
                innerHtml = string.IsNullOrEmpty(innerHtml) ? PageUtils.GetFileNameFromUrl(fileUrl) : innerHtml;

                if (isLower)
                {
                    innerHtml = StringUtils.ToLower(innerHtml);
                }
                if (isUpper)
                {
                    innerHtml = StringUtils.ToUpper(innerHtml);
                }

                retVal = $@"<a {TranslateUtils.ToAttributesString(linkAttributes)}>{innerHtml}</a>";
            }

            return retVal;
        }
    }
}
