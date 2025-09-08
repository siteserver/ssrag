from repositories import site_repository
from .__base import GetResult
from configs import constants


async def settings_get(siteId: int) -> GetResult:
    site = site_repository.get(siteId)
    if site is None:
        return GetResult(settings=None)

    site_values = site.site_values
    site_values.separators = (
        site_values.separators or constants.DEFAULT_CHUNK_SEPARATORS
    )
    site_values.chunkSize = site_values.chunkSize or constants.DEFAULT_CHUNK_SIZE
    site_values.chunkOverlap = (
        site_values.chunkOverlap or constants.DEFAULT_CHUNK_OVERLAP
    )
    site_values.isChunkReplaces = (
        site_values.isChunkReplaces or constants.DEFAULT_CHUNK_IS_CHUNK_REPLACES
    )
    site_values.isChunkDeletes = (
        site_values.isChunkDeletes or constants.DEFAULT_CHUNK_IS_CHUNK_DELETES
    )

    return GetResult(settings=site_values)
