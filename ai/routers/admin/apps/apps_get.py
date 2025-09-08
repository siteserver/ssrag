from repositories import site_repository
from .__base import GetResult


async def apps_get() -> GetResult:
    site_repository.clear_cache()
    sites = site_repository.get_summaries()
    rootSiteId = 0
    for site in sites:
        if site.root:
            rootSiteId = site.id
            break

    return GetResult(sites=sites, rootSiteId=rootSiteId)
