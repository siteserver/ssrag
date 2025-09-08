from repositories import document_repository
from .__base import GetMarkdownRequest
from fastapi import HTTPException
from dto import StringResult
from storages import Storage
from utils import file_utils


async def documents_get_markdown(request: GetMarkdownRequest) -> StringResult:
    document = document_repository.get_document(request.documentId)
    if document is None:
        raise HTTPException(status_code=400, detail="未找到文档")

    storage = Storage()
    file_path = file_utils.combine_url(document.dirPath, f"{document.uuid}.md")
    content = storage.load_text(file_path)

    return StringResult(value=content)
