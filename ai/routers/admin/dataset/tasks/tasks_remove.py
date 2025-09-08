from .__base import RemoveRequest
from dto import BoolResult
from services import dataset_manager


async def tasks_remove(request: RemoveRequest) -> BoolResult:
    if request.contentId and request.channelId:
        dataset_manager.remove_content(
            request.siteId, request.channelId, request.contentId
        )
    elif request.channelId:
        dataset_manager.remove_channel(request.siteId, request.channelId)
    else:
        dataset_manager.remove_all(request.siteId)

    return BoolResult(value=True)
