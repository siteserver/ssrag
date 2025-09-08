from fastapi import APIRouter
from configs import router_prefix
from .__base import InsertRequest, UpdateRequest, GetRequest, GetResult, InsertResult
from .segments_get import segments_get
from .segments_update import segments_update
from .segments_insert import segments_insert
from dto import BoolResult

router = APIRouter(
    prefix=router_prefix.ADMIN_DATASET_SEGMENTS, tags=["dataset/segments"]
)


@router.get("")
async def get(
    siteId: int,
    channelId: int,
    documentId: int,
    page: int = 1,
    pageSize: int = 10,
) -> GetResult:
    request = GetRequest(
        siteId=siteId,
        channelId=channelId,
        documentId=documentId,
        page=page,
        pageSize=pageSize,
    )
    return await segments_get(request)


@router.post("/actions/update")
async def update(request: UpdateRequest) -> BoolResult:
    """更新切片"""
    return segments_update(request)


@router.post("/actions/insert")
async def insert(request: InsertRequest) -> InsertResult:
    """新增切片"""
    return segments_insert(request)
