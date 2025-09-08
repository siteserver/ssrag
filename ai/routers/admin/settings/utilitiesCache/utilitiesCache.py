from fastapi import APIRouter
from dto import BoolResult
from .utilitiesCache_clearCache import utilitiesCache_clearCache
from configs import router_prefix

router = APIRouter(
    prefix=router_prefix.ADMIN_SETTINGS_UTILITIES_CACHE,
    tags=["admin/settings/utilitiesCache"],
)


@router.post("/actions/clearCache")
async def clearCache() -> BoolResult:
    return utilitiesCache_clearCache()
