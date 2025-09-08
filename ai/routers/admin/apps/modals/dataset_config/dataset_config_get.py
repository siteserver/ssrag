from fastapi import HTTPException
from utils import string_utils
from repositories import channel_repository, dataset_repository
from .__base import GetResult


async def dataset_config_get(siteId: int, nodeId: str, datasetSiteId: int) -> GetResult:
    dataset = dataset_repository.get(siteId, nodeId, datasetSiteId)
    if dataset is None:
        raise HTTPException(status_code=400, detail="Dataset not found")

    channels = channel_repository.get_channel_options(datasetSiteId, 0)

    datasetChannelIds: list[list[int]] = []
    if not dataset.datasetAllChannels:
        channelIds = string_utils.split_to_int(dataset.datasetChannelIds)
        for channelId in channelIds:
            channel = channel_repository.get(datasetSiteId, channelId)
            if channel:
                array = string_utils.split_to_int(
                    channel.parentsPath + "," + str(channelId)
                )
                datasetChannelIds.append(array)

    return GetResult(
        dataset=dataset, channels=channels, datasetChannelIds=datasetChannelIds
    )
