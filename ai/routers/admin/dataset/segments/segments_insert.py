from .__base import InsertRequest, InsertResult
from repositories import document_repository, segment_repository
from fastapi import HTTPException
from models import Segment
from vectors import Vector
import uuid


def segments_insert(request: InsertRequest) -> InsertResult:
    if not request.content:
        return InsertResult(segment=None)

    document = document_repository.get_document(request.documentId)
    if document is None:
        raise HTTPException(status_code=400, detail="Document not found")

    segment_repository.update_all_taxis(document.id, request.taxis)

    taxis = request.taxis + 1
    segment = Segment(
        id=str(uuid.uuid4()),
        siteId=document.siteId,
        channelId=document.channelId,
        contentId=document.contentId,
        documentId=document.id,
        taxis=taxis,
        text=request.content,
        textHash="",
    )

    vector = Vector()
    vector.add_segments(document, [segment])

    return InsertResult(segment=segment)
