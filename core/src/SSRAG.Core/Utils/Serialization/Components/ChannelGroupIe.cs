﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Core.Utils.Serialization.Atom.Atom.Core;
using SSRAG.Models;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Dto;

namespace SSRAG.Core.Utils.Serialization.Components
{
    public class ChannelGroupIe
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly Caching _caching;

        public ChannelGroupIe(IDatabaseManager databaseManager, Caching caching)
        {
            _databaseManager = databaseManager;
            _caching = caching;
        }

        public AtomEntry Export(ChannelGroup group)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "IsNodeGroup", true.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(ChannelGroup.GroupName), "NodeGroupName" }, group.GroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ChannelGroup.Taxis), group.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ChannelGroup.Description), group.Description);

            return entry;
        }

        public async Task ImportAsync(AtomFeed feed, int siteId, string uuid)
        {
            var groups = new List<ChannelGroup>();

            foreach (AtomEntry entry in feed.Entries)
            {
                var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsNodeGroup"));
                if (!isNodeGroup) continue;

                var groupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(ChannelGroup.GroupName), "NodeGroupName" });
                if (string.IsNullOrEmpty(groupName)) continue;
                if (await _databaseManager.ChannelGroupRepository.IsExistsAsync(siteId, groupName)) continue;

                var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ChannelGroup.Taxis)));
                var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ChannelGroup.Description));

                groups.Add(new ChannelGroup
                {
                    GroupName = groupName,
                    SiteId = siteId,
                    Taxis = taxis,
                    Description = description
                });
            }

            foreach (var group in groups)
            {
                await _caching.SetProcessAsync(uuid, $"导入栏目组: {group.GroupName}");
                await _databaseManager.ChannelGroupRepository.InsertAsync(group);
            }
        }
    }
}
