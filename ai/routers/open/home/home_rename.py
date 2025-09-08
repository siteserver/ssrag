from repositories import chat_group_repository
from dto import BoolResult
from .__base import RenameRequest


async def home_rename(request: RenameRequest) -> BoolResult:
    chat_group_repository.update_title(request.siteId, request.sessionId, request.title)

    return BoolResult(value=True)
