from repositories import channel_repository, document_repository
from utils import string_utils
from .__base import ListRequest, ListResult


async def documents_list(request: ListRequest) -> ListResult:
    channelId = request.channelId
    if request.channelId == request.siteId:
        channelId = 0

    total = document_repository.get_total_count(
        request.siteId,
        channelId,
        request.contentId,
        request.search,
    )

    documents = document_repository.get_documents(
        request.siteId,
        channelId,
        request.contentId,
        request.search,
        (request.page - 1) * 28,
        28,
    )

    channelIds: list[int] = [request.siteId]
    if channelId != 0:
        channel = channel_repository.get(request.siteId, channelId)
        if channel:
            array = string_utils.split_to_int(
                channel.parentsPath + "," + str(channelId)
            )
            channelIds = array

    return ListResult(documents=documents, total=total, channelIds=channelIds)
