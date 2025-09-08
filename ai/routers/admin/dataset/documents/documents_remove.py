# from services import vector_manager
from dto import BoolResult
from .__base import RemoveRequest
from repositories import document_repository, segment_repository
from vectors import Vector


async def documents_remove(request: RemoveRequest) -> BoolResult:
    # vector_manager.delete_document(request.documentId, True)
    document_repository.delete_by_id(request.documentId)
    segment_repository.delete_by_document_id(request.documentId)
    vector = Vector()
    vector.delete_by_document_id(request.documentId)
    return BoolResult(value=True)
