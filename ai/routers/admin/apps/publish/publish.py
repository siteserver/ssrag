from fastapi import APIRouter
from .__base import GetRequest, GetResult
from configs import router_prefix
from fastapi import UploadFile
from dto import StringResult
from .publish_get import publish_get
from .publish_upload import publish_upload

router = APIRouter(prefix=router_prefix.ADMIN_APPS_PUBLISH, tags=["apps/publish"])


@router.get("")
async def get(siteId: int) -> GetResult:
    request = GetRequest(
        siteId=siteId,
    )
    return await publish_get(request)


@router.post("/actions/upload")
async def upload(file: UploadFile) -> StringResult:
    return await publish_upload(file)
