from fastapi import APIRouter
from configs import router_prefix
from dto import BoolResult
from repositories import model_provider_repository
from enums import ProviderType
from pydantic import BaseModel
from configs import app_configs

router = APIRouter(prefix=router_prefix.UPGRADE, tags=["upgrade"])

class SubmitRequest(BaseModel):
    securityKey: str = ""

@router.post("")
async def upgrade(request: SubmitRequest) -> BoolResult:
    if request.securityKey != app_configs.SECURITY_KEY:
        return BoolResult(value=False)
    
    provider = model_provider_repository.get_by_provider_id(ProviderType.SSRAG)
    if provider and provider.iconUrl == "ssrag_square.svg":
        provider.iconUrl = "ssrag_square.png"
        model_provider_repository.update(provider)
    
    return BoolResult(value=True)

