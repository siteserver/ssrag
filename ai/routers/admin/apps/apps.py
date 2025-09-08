from fastapi import APIRouter
from .apps_get import apps_get
from .apps_init import apps_init
from .__base import (
    InitRequest,
    GetResult,
    DisableRequest,
    OrderRequest,
    DeleteAIRequest,
)
from configs import router_prefix
from .apps_upload import apps_upload
from .apps_disable import apps_disable
from .apps_order import apps_order
from .apps_delete_ai import apps_delete_ai
from .apps_clear_cache import apps_clear_cache
from fastapi import UploadFile
from dto import StringResult, BoolResult

router = APIRouter(prefix=router_prefix.ADMIN_APPS, tags=["apps"])


@router.get("")
async def get() -> GetResult:
    return await apps_get()


@router.post("/actions/init")
async def submit(request: InitRequest) -> BoolResult:
    return await apps_init(request)


@router.post("/actions/upload")
async def upload(file: UploadFile) -> StringResult:
    return await apps_upload(file)


@router.post("/actions/disable")
async def disable(request: DisableRequest) -> BoolResult:
    return await apps_disable(request)


@router.post("/actions/order")
async def order(request: OrderRequest) -> BoolResult:
    return await apps_order(request)


@router.post("/actions/deleteAI")
async def delete_ai(request: DeleteAIRequest) -> BoolResult:
    return await apps_delete_ai(request)


@router.post("/actions/clearCache")
async def clear_cache() -> BoolResult:
    return await apps_clear_cache()
