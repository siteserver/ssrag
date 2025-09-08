﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Core.Utils.Serialization.Atom.Atom.Core;
using SSRAG.Models;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Core.Utils.Serialization.Components
{
    public class ContentTagIe
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly Caching _caching;

        public ContentTagIe(IDatabaseManager databaseManager, Caching caching)
        {
            _databaseManager = databaseManager;
            _caching = caching;
        }

        public AtomEntry Export(ContentTag group)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "IsContentTag", true.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentTag.TagName), group.TagName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentTag.SiteId), group.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentTag.ContentIds), ListUtils.ToString(group.ContentIds));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentTag.UseNum), group.UseNum.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentTag.Level), group.Level.ToString());

            return entry;
        }

        public async Task ImportAsync(AtomFeed feed, int siteId, string uuid)
        {
            var contentTags = new List<ContentTag>();

            var tagNames = await _databaseManager.ContentTagRepository.GetTagNamesAsync(siteId);

            foreach (AtomEntry entry in feed.Entries)
            {
                var isContentTag = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsContentTag"));
                if (!isContentTag) continue;

                var tagName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentTag.TagName));
                if (string.IsNullOrEmpty(tagName) || ListUtils.Contains(tagNames, tagName)) continue;

                var useNum = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentTag.UseNum)));
                var level = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentTag.Level)));
                contentTags.Add(new ContentTag
                {
                    TagName = tagName,
                    SiteId = siteId,
                    UseNum = useNum,
                    Level = level
                });
            }

            foreach (var contentTag in contentTags)
            {
                await _caching.SetProcessAsync(uuid, $"导入内容标签: {contentTag.TagName}");
                await _databaseManager.ContentTagRepository.InsertAsync(contentTag);
            }
        }
    }
}
