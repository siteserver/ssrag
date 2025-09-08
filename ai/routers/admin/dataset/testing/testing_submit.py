from vectors import Vector
from repositories import channel_repository
from .__base import SubmitRequest, SubmitResult
from dto import SearchScope
from utils import string_utils


async def testing_submit(request: SubmitRequest) -> SubmitResult:
    searchScope = SearchScope(siteIds=[], channelIds=[], contentIds=[])
    if request.contentId:
        searchScope.contentIds.append(request.contentId)
    elif request.channelId:
        channel_ids = channel_repository.get_channel_ids_with_self(request.channelId)
        searchScope.channelIds.extend(channel_ids)
    elif request.siteId:
        searchScope.siteIds.append(request.siteId)

    cuts = string_utils.jieba_cut_to_list(request.query)

    vector = Vector()
    results = vector.search(
        searchScope,
        request.query,
        request.searchType,
        request.maxCount,
        request.minScore,
    )

    channelId = request.channelId
    if request.channelId == request.siteId:
        channelId = 0

    channelIds: list[int] = [request.siteId]
    if channelId != 0:
        channel = channel_repository.get(request.siteId, channelId)
        if channel:
            array = string_utils.split_to_int(
                channel.parentsPath + "," + str(channelId)
            )
            channelIds = array

    return SubmitResult(cuts=cuts, results=results, channelIds=channelIds)
