from enums import SiteType
from repositories import dataset_repository, site_repository
from .__base import GetResult
from fastapi import HTTPException


async def dataset_select_get(siteId: int, nodeId: str) -> GetResult:
    sites = site_repository.get_summaries()
    sites = [
        site
        for site in sites
        if site.siteType == SiteType.WEB
        or site.siteType == SiteType.MARKDOWN
        or site.siteType == SiteType.DOCUMENT
    ]

    site = site_repository.get(siteId)
    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")
    if not nodeId:
        nodeId = site.uuid

    selected_site_ids = dataset_repository.get_selected_site_ids(siteId, nodeId)

    return GetResult(sites=sites, selectedSiteIds=selected_site_ids)
