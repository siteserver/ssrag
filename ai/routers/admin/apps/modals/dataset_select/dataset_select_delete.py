from .__base import DeleteRequest
from repositories import (
    dataset_repository,
    site_repository,
)
from dto import BoolResult
from fastapi import HTTPException


async def dataset_select_delete(request: DeleteRequest) -> BoolResult:
    site = site_repository.get(request.siteId)
    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    nodeId = request.nodeId
    if not nodeId:
        nodeId = site.uuid

    dataset_repository.delete(request.siteId, nodeId, request.datasetSiteId)

    return BoolResult(value=True)
