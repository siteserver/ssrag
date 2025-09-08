from fastapi import APIRouter
from .dataset_config_get import dataset_config_get
from .dataset_config_submit import dataset_config_submit
from .__base import GetResult, SubmitRequest
from configs import router_prefix
from dto import BoolResult

router = APIRouter(
    prefix=router_prefix.ADMIN_APPS_MODALS_DATASET_CONFIG,
    tags=["datasetConfig"],
)


@router.get("")
async def get(siteId: int, nodeId: str, datasetSiteId: int) -> GetResult:
    return await dataset_config_get(siteId, nodeId, datasetSiteId)


@router.post("")
async def submit(request: SubmitRequest) -> BoolResult:
    return await dataset_config_submit(request)
