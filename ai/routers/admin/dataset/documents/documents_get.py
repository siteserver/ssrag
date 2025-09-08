from .__base import GetRequest, GetResult
from repositories import channel_repository, config_repository, site_repository


async def documents_get(request: GetRequest) -> GetResult:
    isModelReady = True
    configs = config_repository.get_values()
    if (
        configs.defaultTextEmbeddingModelId is None
        or configs.defaultTextEmbeddingProviderId is None
    ):
        isModelReady = False

    site = site_repository.get(request.siteId)
    siteUrl = "/"
    if site is not None:
        siteUrl = f"/{site.siteDir}/"

    channels = channel_repository.get_channel_options(request.siteId, 0)

    return GetResult(isModelReady=isModelReady, siteUrl=siteUrl, channels=channels)
