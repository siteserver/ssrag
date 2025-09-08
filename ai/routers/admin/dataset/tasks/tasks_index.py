from .__base import IndexRequest
from dto import BoolResult
from services import dataset_manager


async def tasks_index(request: IndexRequest) -> BoolResult:
    if request.contentId and request.channelId:
        dataset_manager.index_content(
            request.siteId, request.channelId, request.contentId
        )
    elif request.channelId:
        dataset_manager.index_channel(request.siteId, request.channelId)
    else:
        dataset_manager.index_all(request.siteId)

    return BoolResult(value=True)
