from .__base import OrderRequest
from fastapi import HTTPException
from repositories import site_repository
from dto import BoolResult


async def apps_order(request: OrderRequest) -> BoolResult:
    for i in range(request.rows):
        site = site_repository.get(request.siteId)
        if site is None:
            raise HTTPException(status_code=404, detail="Site not found")

        if request.isUp:
            site_repository.update_taxis_up(site.id, site.taxis)
        else:
            site_repository.update_taxis_down(site.id, site.taxis)

    return BoolResult(value=True)
