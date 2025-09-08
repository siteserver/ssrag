from fastapi import APIRouter
from .logs_get import logs_get
from .logs_messages import logs_messages
from .__base import GetRequest, GetResponse, MessageRequest, MessageResponse
from configs import router_prefix

router = APIRouter(prefix=router_prefix.ADMIN_APPS_LOGS, tags=["apps/logs"])


@router.get("")
async def get(
    siteId: int,
    page: int = 1,
    pageSize: int = 10,
    dateStart: str | None = None,
    dateEnd: str | None = None,
    title: str | None = None,
) -> GetResponse:
    request = GetRequest(
        siteId=siteId,
        page=page,
        pageSize=pageSize,
        dateStart=dateStart,
        dateEnd=dateEnd,
        title=title,
    )
    return await logs_get(request)


@router.post("/actions/messages")
async def messages(request: MessageRequest) -> MessageResponse:
    return await logs_messages(request)
