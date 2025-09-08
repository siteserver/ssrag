from fastapi import APIRouter, Depends
from managers import AuthManager
from .chat_get import chat_get
from .chat_submit import chat_submit
from .chat_history import chat_history
from .__base import SubmitRequest, GetResult, HistoryRequest
from configs import router_prefix

router = APIRouter(prefix=router_prefix.OPEN_CHAT, tags=["open/chat"])


@router.get("")
async def get(
    id: str | None = None, siteId: int | None = None, sessionId: str | None = None
) -> GetResult:
    return await chat_get(id, siteId, sessionId)


@router.post("")
async def submit(request: SubmitRequest):
    return await chat_submit(request)


@router.post("/actions/history")
async def history(
    request: HistoryRequest,
    auth_manager: AuthManager = Depends(AuthManager.get_auth_manager),
):
    return await chat_history(auth_manager, request)
