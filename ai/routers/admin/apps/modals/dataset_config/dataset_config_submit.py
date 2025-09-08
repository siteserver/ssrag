from .__base import SubmitRequest
from fastapi import HTTPException
from repositories import (
    dataset_repository,
)
from dto import BoolResult
from utils import string_utils


async def dataset_config_submit(request: SubmitRequest) -> BoolResult:
    dataset = dataset_repository.get(
        request.siteId, request.nodeId, request.datasetSiteId
    )
    if dataset is None:
        raise HTTPException(status_code=400, detail="Dataset not found")

    dataset.datasetAllChannels = request.datasetAllChannels
    datasetChannelIds = []
    for channelIds in request.datasetChannelIds:
        channelId = channelIds[len(channelIds) - 1]
        datasetChannelIds.append(channelId)
    dataset.datasetChannelIds = string_utils.join(datasetChannelIds)

    dataset_repository.update(dataset)

    return BoolResult(value=True)
