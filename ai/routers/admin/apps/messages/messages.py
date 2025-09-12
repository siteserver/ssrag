from fastapi import APIRouter
from .messages_get import messages_get
from .messages_messages import messages_messages
from .__base import GetRequest, GetResponse, MessageRequest, MessageResponse
from configs import router_prefix

router = APIRouter(prefix=router_prefix.ADMIN_APPS_MESSAGES, tags=["apps/messages"])


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
    return await messages_get(request)


@router.post("/actions/messages")
async def messages(request: MessageRequest) -> MessageResponse:
    return await messages_messages(request)
