from repositories import channel_repository, segment_repository
from .__base import GetRequest, GetResult


async def segments_get(request: GetRequest) -> GetResult:
    count = segment_repository.count(request.documentId)
    segments = segment_repository.get_all_by_document_id_and_page(
        request.documentId, request.page, request.pageSize
    )

    breadcrumb = channel_repository.get_breadcrumb(
        request.siteId,
        request.channelId if request.channelId != 0 else request.siteId,
    )

    return GetResult(segments=segments, count=count, breadcrumb=breadcrumb)
