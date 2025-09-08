from fastapi import APIRouter
from .__base import (
    GetRequest,
    GetResult,
    SubmitRequest,
    SubmitResult,
)
from configs import router_prefix

router = APIRouter(prefix=router_prefix.ADMIN_DATASET_TESTING, tags=["dataset/testing"])


@router.get("")
async def get(siteId: int) -> GetResult:
    from .testing_get import testing_get

    request = GetRequest(siteId=siteId)
    return await testing_get(request)


@router.post("")
async def submit(request: SubmitRequest) -> SubmitResult:
    from .testing_submit import testing_submit

    return await testing_submit(request)
