from repositories import site_repository, config_repository
from services import AppManager
from .__base import GetResult


async def apps_get() -> GetResult:
    site_repository.clear_cache()
    sites = site_repository.get_summaries()
    rootSiteId = 0
    for site in sites:
        if site.root:
            rootSiteId = site.id
            break
          
    config_values = config_repository.get_values()
    if not config_values.init:
        AppManager.initialize(config_values)

    return GetResult(sites=sites, rootSiteId=rootSiteId)
