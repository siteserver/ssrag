from repositories import chat_group_repository
from .__base import GetRequest, GetResponse


async def logs_get(request: GetRequest) -> GetResponse:
    chatGroups = chat_group_repository.get_all_by_site_id(
        request.siteId,
        request.page,
        request.pageSize,
        request.dateStart,
        request.dateEnd,
        request.title,
    )

    return GetResponse(chatGroups=chatGroups)
