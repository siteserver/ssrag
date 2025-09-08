from repositories import chat_group_repository
from dto import BoolResult
from .__base import DeleteRequest


async def home_delete(request: DeleteRequest) -> BoolResult:
    chat_group_repository.update_deleted(request.siteId, request.sessionId)

    return BoolResult(value=True)
