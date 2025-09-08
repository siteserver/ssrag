from .__base import DisableRequest
from fastapi import HTTPException
from repositories import site_repository
from dto import BoolResult


async def apps_disable(request: DisableRequest) -> BoolResult:
    site = site_repository.get(request.siteId)
    if site is None:
        raise HTTPException(status_code=400, detail="Site not found")

    site.disabled = not site.disabled
    site_repository.update_site(site)

    return BoolResult(value=True)
