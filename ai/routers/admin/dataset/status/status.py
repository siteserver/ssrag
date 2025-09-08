from fastapi import APIRouter
from .__base import GetResult
from configs import router_prefix


router = APIRouter(prefix=router_prefix.ADMIN_DATASET_STATUS, tags=["dataset/status"])


@router.get("")
async def get(siteId: int) -> GetResult:
    from .status_get import status_get

    return await status_get(siteId)
