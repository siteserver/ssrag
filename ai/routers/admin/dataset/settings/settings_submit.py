from .__base import SubmitRequest
from dto import BoolResult
from repositories import site_repository
from fastapi import HTTPException
from services import dataset_manager


async def settings_submit(request: SubmitRequest) -> BoolResult:
    site = site_repository.get(request.siteId)
    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    site_values = site.site_values
    site_values.separators = request.separators
    site_values.chunkSize = request.chunkSize
    site_values.chunkOverlap = request.chunkOverlap
    site_values.isChunkReplaces = request.isChunkReplaces
    site_values.isChunkDeletes = request.isChunkDeletes

    site.site_values = site_values
    site_repository.update_site(site)

    if request.isRechunk:
        dataset_manager.remove_all(request.siteId)
        dataset_manager.index_all(request.siteId)

    return BoolResult(value=True)
