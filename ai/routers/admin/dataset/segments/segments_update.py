from .__base import UpdateRequest
from vectors import Vector
from dto import BoolResult


def segments_update(request: UpdateRequest) -> BoolResult:
    vector = Vector()

    if (request.content is None) or (request.content == ""):
        vector.delete_by_ids([request.segmentId])
    else:
        vector.update(request.segmentId, request.content)

    return BoolResult(value=True)
