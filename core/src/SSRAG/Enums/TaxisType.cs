﻿using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaxisType
    {
        [DataEnum(DisplayName = "内容ID（升序）")] OrderById,
        [DataEnum(DisplayName = "内容ID（降序）")] OrderByIdDesc,
        [DataEnum(DisplayName = "栏目ID（升序）")] OrderByChannelId,
        [DataEnum(DisplayName = "栏目ID（降序）")] OrderByChannelIdDesc,
        [DataEnum(DisplayName = "添加时间（升序）")] OrderByAddDate,
        [DataEnum(DisplayName = "添加时间（降序）")] OrderByAddDateDesc,
        [DataEnum(DisplayName = "内容标题（升序）")] OrderByTitle,
        [DataEnum(DisplayName = "内容标题（降序）")] OrderByTitleDesc,
        [DataEnum(DisplayName = "更新时间（升序）")] OrderByLastModifiedDate,
        [DataEnum(DisplayName = "更新时间（降序）")] OrderByLastModifiedDateDesc,
        [DataEnum(DisplayName = "默认排序（升序）")] OrderByTaxis,
        [DataEnum(DisplayName = "默认排序")] OrderByTaxisDesc,
        [DataEnum(DisplayName = "按点击量排序")] OrderByHits,
        [DataEnum(DisplayName = "随机排序")] OrderByRandom
    }
}
