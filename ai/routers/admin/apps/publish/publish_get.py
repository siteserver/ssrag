from repositories import site_repository
from .__base import GetRequest, GetResult
from fastapi import HTTPException


async def publish_get(request: GetRequest) -> GetResult:
    site = site_repository.get(request.siteId)

    if not site:
        raise HTTPException(status_code=404, detail="Site not found")

    return GetResult(site=site, values=site.site_values)
