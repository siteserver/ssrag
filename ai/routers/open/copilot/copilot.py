from fastapi import APIRouter, Depends
from managers import AuthManager
from .copilot_get import copilot_get
from .__base import GetResult
from configs import router_prefix


router = APIRouter(prefix=router_prefix.OPEN_COPILOT, tags=["open/copilot"])


@router.get("")
async def get(
    id: str | None = None,
    siteId: int | None = None,
    sessionId: str | None = None,
    auth_manager: AuthManager = Depends(AuthManager.get_auth_manager),
) -> GetResult:
    return await copilot_get(auth_manager, id, siteId, sessionId)
