from repositories import document_repository
from fastapi import HTTPException
from .__base import ChunkAndEmbedRequest, ChunkAndEmbedResult
from repositories import segment_repository
from utils import md_utils
from vectors import Vector
from storages import Storage
from utils import file_utils


async def documents_chunk_and_embed(
    request: ChunkAndEmbedRequest,
) -> ChunkAndEmbedResult:
    vector = Vector()

    segment_ids = segment_repository.get_segment_ids_by_document_ids(
        [request.documentId]
    )
    vector.delete_by_ids(segment_ids)
    document = document_repository.get_document(request.documentId)
    if document is None:
        raise HTTPException(status_code=404, detail="Document not found")
    document.separators = ",".join(request.config.separators)
    document.chunkSize = request.config.chunkSize
    document.chunkOverlap = request.config.chunkOverlap
    document.isChunkReplaces = request.config.isChunkReplaces
    document.isChunkDeletes = request.config.isChunkDeletes
    document_repository.update(document)

    storage = Storage()
    file_path = file_utils.combine_url(document.dirPath, f"{document.uuid}.md")

    md_content = storage.load_text(file_path)
    chunks = md_utils.chunk_document_md(document, md_content)
    vector.add_texts(document, chunks)

    return ChunkAndEmbedResult(document=document)
