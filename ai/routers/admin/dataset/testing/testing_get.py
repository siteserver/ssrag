from .__base import GetRequest, GetResult
from repositories import channel_repository, site_repository


async def testing_get(request: GetRequest) -> GetResult:
    site = site_repository.get(request.siteId)
    siteUrl = "/"
    if site is not None:
        siteUrl = f"/{site.siteDir}/"

    channels = channel_repository.get_channel_options(request.siteId, 0)

    return GetResult(siteUrl=siteUrl, channels=channels)
