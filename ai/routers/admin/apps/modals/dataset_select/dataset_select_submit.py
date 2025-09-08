from .__base import SubmitRequest
from fastapi import HTTPException
from repositories import (
    dataset_repository,
    site_repository,
)
from models import Dataset
from dto import BoolResult


async def dataset_select_submit(request: SubmitRequest) -> BoolResult:
    site = site_repository.get(request.siteId)
    if site is None:
        raise HTTPException(status_code=400, detail="Site not found")

    nodeId = request.nodeId
    if not nodeId:
        nodeId = site.uuid

    if dataset_repository.exists(request.siteId, nodeId, request.datasetSiteId):
        dataset_repository.delete(request.siteId, nodeId, request.datasetSiteId)
    else:
        dataset = Dataset(
            siteId=request.siteId,
            nodeId=nodeId,
            datasetSiteId=request.datasetSiteId,
            datasetAllChannels=True,
            datasetChannelIds="",
        )
        dataset_repository.insert(dataset)

    return BoolResult(value=True)
