from fastapi import APIRouter
from .dataset_select_get import dataset_select_get
from .dataset_select_submit import dataset_select_submit
from .dataset_select_delete import dataset_select_delete
from .__base import GetResult, SubmitRequest, DeleteRequest
from configs import router_prefix
from dto import BoolResult

router = APIRouter(
    prefix=router_prefix.ADMIN_APPS_MODALS_DATASET_SELECT,
    tags=["datasetSelect"],
)


@router.get("")
async def get(siteId: int, nodeId: str) -> GetResult:
    return await dataset_select_get(siteId, nodeId)


@router.post("")
async def submit(request: SubmitRequest) -> BoolResult:
    return await dataset_select_submit(request)


@router.post("/actions/delete")
async def delete(request: DeleteRequest) -> BoolResult:
    return await dataset_select_delete(request)
