from fastapi import APIRouter, Depends
from managers import AuthManager
from .home_get import home_get
from .home_delete import home_delete
from .home_rename import home_rename
from .__base import GetResult, DeleteRequest, RenameRequest
from configs import router_prefix


router = APIRouter(prefix=router_prefix.OPEN_HOME, tags=["open/home"])


@router.get("")
async def get(
    id: str | None = None,
    siteId: int | None = None,
    sessionId: str | None = None,
    auth_manager: AuthManager = Depends(AuthManager.get_auth_manager),
) -> GetResult:
    return await home_get(auth_manager, id, siteId, sessionId)


@router.post("/actions/delete")
async def delete(request: DeleteRequest):
    return await home_delete(request)


@router.post("/actions/rename")
async def rename(request: RenameRequest):
    return await home_rename(request)
