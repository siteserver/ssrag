from fastapi import APIRouter
from .configsModels_get import configsModels_get
from .configsModels_get_manifests import configsModels_get_manifests
from .configsModels_submit_provider import configsModels_submit_provider
from .configsModels_submit_model import configsModels_submit_model
from .configsModels_delete_provider import configsModels_delete_provider
from .configsModels_delete_model import configsModels_delete_model
from .configsModels_get_default import configsModels_get_default
from .configsModels_submit_default import configsModels_submit_default
from .configsModels_status import configsModels_status
from .__base import (
    GetManifestsResult,
    SubmitProviderRequest,
    SubmitModelRequest,
    GetResult,
    DeleteProviderRequest,
    DeleteModelRequest,
    GetDefaultsResult,
    SubmitDefaultsRequest,
    StatusRequest,
    StatusResult,
)
from configs import router_prefix
from dto import BoolResult, StringResult

router = APIRouter(
    prefix=router_prefix.ADMIN_SETTINGS_CONFIGS_MODELS,
    tags=["admin/settings/configsModels"],
)


@router.get("")
async def get() -> GetResult:
    return await configsModels_get()


@router.get("/manifests")
async def getManifests() -> GetManifestsResult:
    return await configsModels_get_manifests()


@router.post("/actions/submitProvider")
async def provider(request: SubmitProviderRequest) -> BoolResult:
    return await configsModels_submit_provider(request)


@router.post("/actions/submitModel")
async def model(request: SubmitModelRequest) -> BoolResult:
    return await configsModels_submit_model(request)


@router.post("/actions/deleteProvider")
async def deleteProvider(request: DeleteProviderRequest) -> BoolResult:
    return await configsModels_delete_provider(request)


@router.post("/actions/deleteModel")
async def deleteModel(request: DeleteModelRequest) -> BoolResult:
    return await configsModels_delete_model(request)


@router.post("/actions/getDefaults")
async def getDefaults() -> GetDefaultsResult:
    return await configsModels_get_default()


@router.post("/actions/submitDefaults")
async def submitDefaults(request: SubmitDefaultsRequest) -> StringResult:
    return await configsModels_submit_default(request)


@router.post("/actions/status")
async def status(request: StatusRequest) -> StatusResult:
    return configsModels_status(request)
