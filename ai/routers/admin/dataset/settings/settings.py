from fastapi import APIRouter
from .__base import GetResult, SubmitRequest
from configs import router_prefix
from dto import BoolResult


router = APIRouter(
    prefix=router_prefix.ADMIN_DATASET_SETTINGS, tags=["dataset/settings"]
)


@router.get("")
async def get(siteId: int) -> GetResult:
    from .settings_get import settings_get

    return await settings_get(siteId)


@router.post("")
async def submit(request: SubmitRequest) -> BoolResult:
    from .settings_submit import settings_submit

    return await settings_submit(request)
