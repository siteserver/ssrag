# from services import vector_manager
from dto import BoolResult
from .__base import RemoveRequest
from repositories import document_repository, segment_repository
from vectors import Vector
from storages import Storage
from fastapi import HTTPException


async def documents_remove(request: RemoveRequest) -> BoolResult:
    # vector_manager.delete_document(request.documentId, True)
    document = document_repository.get_document(request.documentId)
    if document is None:
        raise HTTPException(status_code=404, detail="Document not found")
      
    document_repository.delete_by_id(request.documentId)
    segment_repository.delete_by_document_id(request.documentId)
    vector = Vector()
    vector.delete_by_document_id(request.documentId)
    storage = Storage()
    storage.delete_by_prefix(f"{document.dirPath}/{document.uuid}")
    return BoolResult(value=True)
