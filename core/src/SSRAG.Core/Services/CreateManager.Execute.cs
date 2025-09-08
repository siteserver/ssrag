﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSRAG.Configuration;
using SSRAG.Core.StlParser.StlElement;
using SSRAG.Core.StlParser.Utility;
using SSRAG.Core.Utils;
using SSRAG.Enums;
using SSRAG.Models;
using SSRAG.Parse;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class CreateManager
    {
        public async Task ExecuteAsync(int siteId, CreateType createType, int channelId = 0, int contentId = 0,
            int fileTemplateId = 0, int specialId = 0)
        {
            if (createType == CreateType.Index)
            {
                await ExecuteChannelAsync(siteId, siteId);
            }
            else if (createType == CreateType.Channel)
            {
                await ExecuteChannelAsync(siteId, channelId);
            }
            else if (createType == CreateType.Content)
            {
                var site = await _siteRepository.GetAsync(siteId);
                var channel = await _channelRepository.GetAsync(channelId);
                await ExecuteContentAsync(site, channel, contentId);
            }
            else if (createType == CreateType.AllContent)
            {
                await ExecuteContentsAsync(siteId, channelId);
            }
            else if (createType == CreateType.File)
            {
                await ExecuteFileAsync(siteId, fileTemplateId);
            }
            else if (createType == CreateType.Special)
            {
                await ExecuteSpecialAsync(siteId, specialId);
            }
            else if (createType == CreateType.All)
            {
                var channelIdList = await _channelRepository.GetChannelIdsAsync(siteId);
                foreach (var theChannelId in channelIdList)
                {
                    await ExecuteChannelAsync(siteId, theChannelId);
                }

                foreach (var theChannelId in channelIdList)
                {
                    await ExecuteContentsAsync(siteId, theChannelId);
                }

                foreach (var theSpecialId in await _specialRepository.GetAllSpecialIdsAsync(siteId))
                {
                    await ExecuteSpecialAsync(siteId, theSpecialId);
                }

                foreach (var theFileTemplateId in await _templateRepository.GetAllFileTemplateIdsAsync(siteId))
                {
                    await ExecuteFileAsync(siteId, theFileTemplateId);
                }
            }
        }

        private async Task ExecuteContentsAsync(int siteId, int channelId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var channel = await _channelRepository.GetAsync(channelId);
            if (channel.IsCreateBanned) return;

            var contentIdList = await _contentRepository.GetContentIdsCheckedAsync(site, channel);

            foreach (var contentId in contentIdList)
            {
                await ExecuteContentAsync(site, channel, contentId);
            }
        }

        private async Task ExecuteChannelAsync(int siteId, int channelId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var channel = await _channelRepository.GetAsync(channelId);

            var count = await _contentRepository.GetCountOfCheckedAsync(site, channel);
            if (!_channelRepository.IsCreatable(site, channel, count)) return;

            var template = channelId == siteId
                ? await _templateRepository.GetIndexPageTemplateAsync(siteId)
                : await _templateRepository.GetChannelTemplateAsync(siteId, channel);
            var filePath = await _pathManager.GetChannelPageFilePathAsync(site, channelId);

            await _parseManager.InitAsync(EditMode.Default, site, channelId, 0, template, 0);
            _parseManager.ContextInfo.ContextType = ParseType.Channel;

            await ExecuteAsync(site, template, filePath);
        }

        private async Task ExecuteAsync(Site site, Template template, string filePathWithoutPage)
        {
            var contentBuilder = new StringBuilder(await _pathManager.GetTemplateContentAsync(site, template));

            var stlLabelList = ParseUtils.GetStlLabels(contentBuilder.ToString());

            //如果标签中存在<stl:channel type="PageContent"></stl:channel>
            if (StlParserUtility.IsStlChannelElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlChannelElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await _parseManager.ParseInnerContentAsync(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await _parseManager.ParseAsync(contentBuilder, filePathWithoutPage, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var index = pageContentHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await _parseManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, 0);

                    var filePath = _pathManager.GetPageFilePathAsync(filePathWithoutPage, currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + Constants.PagePlaceHolder.Length);
                    }

                    _parseManager.PageInfo = page;
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (ParseUtils.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, _parseManager);
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

                _parseManager.PageInfo.AddPageHeadCodeIfNotExists(ParsePage.Const.Jquery);
                await _parseManager.ParseAsync(contentBuilder, filePathWithoutPage, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await _parseManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, totalNum);

                    var filePath = _pathManager.GetPageFilePathAsync(filePathWithoutPage, currentPageIndex);

                    await GenerateFileAsync(filePath, pagedBuilder);

                    _parseManager.PageInfo = page;
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (ParseUtils.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlElement, _parseManager);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                _parseManager.PageInfo.AddPageHeadCodeIfNotExists(ParsePage.Const.Jquery);
                await _parseManager.ParseAsync(contentBuilder, filePathWithoutPage, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await _parseManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, totalNum);

                    var filePath = _pathManager.GetPageFilePathAsync(filePathWithoutPage, currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    _parseManager.PageInfo = page;
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (ParseUtils.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlElement, _parseManager);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                _parseManager.PageInfo.AddPageHeadCodeIfNotExists(ParsePage.Const.Jquery);
                await _parseManager.ParseAsync(contentBuilder, filePathWithoutPage, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await _parseManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, totalNum);

                    var filePath = _pathManager.GetPageFilePathAsync(filePathWithoutPage, currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    _parseManager.PageInfo = page;
                }
            }
            else
            {
                await _parseManager.ParseAsync(contentBuilder, filePathWithoutPage, false);
                await GenerateFileAsync(filePathWithoutPage, contentBuilder);
            }
        }

        private async Task ExecuteContentAsync(Site site, Channel channel, int contentId)
        {
            if (channel == null || channel.IsCreateBanned) return;

            var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);

            if (contentInfo == null)
            {
                return;
            }

            if (!contentInfo.Checked)
            {
                await DeleteContentAsync(site, channel.Id, contentId);
                return;
            }

            if (!ContentUtility.IsCreatable(channel, contentInfo)) return;

            if (site.IsCreateStaticContentByAddDate &&
                contentInfo.AddDate < site.CreateStaticContentAddDate)
            {
                return;
            }

            var template = await _templateRepository.GetContentTemplateAsync(site.Id, channel, contentInfo.TemplateId);
            await _parseManager.InitAsync(EditMode.Default, site, channel.Id, contentId, template, 0);
            _parseManager.ContextInfo.ContextType = ParseType.Content;
            _parseManager.ContextInfo.SetContent(contentInfo);

            var filePath = await _pathManager.GetContentPageFilePathAsync(site, _parseManager.PageInfo.PageChannelId, contentInfo, 0);
            var contentBuilder = new StringBuilder(await _pathManager.GetTemplateContentAsync(site, template));

            var stlLabelList = ParseUtils.GetStlLabels(contentBuilder.ToString());

            //如果标签中存在<stl:content type="PageContent"></stl:content>
            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await _parseManager.ParseInnerContentAsync(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await _parseManager.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var index = pageContentHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(site, page.PageChannelId, contentInfo, currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + Constants.PagePlaceHolder.Length);
                    }

                    _parseManager.PageInfo = page;
                }
            }
            //如果标签中存在<stl:channel type="PageContent"></stl:channel>
            else if (StlParserUtility.IsStlChannelElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlChannelElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await _parseManager.ParseInnerContentAsync(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await _parseManager.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var index = pageContentHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(site, page.PageChannelId, contentInfo, currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + Constants.PagePlaceHolder.Length);
                    }

                    _parseManager.PageInfo = page;
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (ParseUtils.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, _parseManager);
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

                await _parseManager.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(site, page.PageChannelId, contentInfo, currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    _parseManager.PageInfo = page;
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (ParseUtils.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlElement, _parseManager);
                var pageCount = pageChannelsElementParser.GetPageCount(out _);

                await _parseManager.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(site, page.PageChannelId, contentInfo, currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    _parseManager.PageInfo = page;
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (ParseUtils.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlElement, _parseManager);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                await _parseManager.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var page = _parseManager.PageInfo;
                    _parseManager.PageInfo = _parseManager.PageInfo.Clone();

                    var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(site, page.PageChannelId, contentInfo, currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    _parseManager.PageInfo = page;
                }
            }
            else //无翻页
            {
                await _parseManager.ParseAsync(contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task ExecuteFileAsync(int siteId, int fileTemplateId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var template = await _templateRepository.GetAsync(fileTemplateId);
            if (template == null || template.TemplateType != TemplateType.FileTemplate)
            {
                return;
            }

            await _parseManager.InitAsync(EditMode.Default, site, siteId, 0, template, 0);

            var filePath = _pathManager.ParseSitePath(site, template.CreatedFileFullName);

            await ExecuteAsync(site, template, filePath);

            //var contentBuilder = new StringBuilder(await _pathManager.GetTemplateContentAsync(site, template));
            //await _parseManager.ParseAsync(contentBuilder, filePath, false);
            //await GenerateFileAsync(filePath, contentBuilder);
        }

        private async Task ExecuteSpecialAsync(int siteId, int specialId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var templates = await _pathManager.GetSpecialTemplateListAsync(site, specialId);

            foreach (var template in templates)
            {
                await _parseManager.InitAsync(EditMode.Default, site, siteId, 0, template, specialId);

                var filePath = _pathManager.ParseSitePath(site, template.CreatedFileFullName);

                var contentBuilder = new StringBuilder(template.Content);
                await _parseManager.ParseAsync(contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task GenerateFileAsync(string filePath, StringBuilder contentBuilder)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                await FileUtils.WriteTextAsync(filePath, contentBuilder.ToString());
            }
            catch
            {
                FileUtils.RemoveReadOnlyAndHiddenIfExists(filePath);
                await FileUtils.WriteTextAsync(filePath, contentBuilder.ToString());
            }
        }
    }
}