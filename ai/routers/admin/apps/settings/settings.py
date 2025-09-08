from fastapi import APIRouter
from .__base import StylesRequest, AiRequest, GetResult
from configs import router_prefix
from fastapi import UploadFile
from dto import StringResult, BoolResult
from .settings_get import settings_get
from .settings_ai import settings_ai
from .settings_styles import settings_styles
from .settings_upload import settings_upload

router = APIRouter(prefix=router_prefix.ADMIN_APPS_SETTINGS, tags=["apps/settings"])


@router.get("")
async def get(siteId: int) -> GetResult:
    return await settings_get(siteId)


@router.post("/actions/styles")
async def styles(request: StylesRequest) -> BoolResult:
    return await settings_styles(request)


@router.post("/actions/ai")
async def ai(request: AiRequest) -> BoolResult:
    return await settings_ai(request)


@router.post("/actions/upload")
async def upload(file: UploadFile) -> StringResult:
    return await settings_upload(file)
