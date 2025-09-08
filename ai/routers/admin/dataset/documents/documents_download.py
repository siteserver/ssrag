from repositories import document_repository
from .__base import DownloadRequest
from fastapi import HTTPException
from dto import StringResult
from storages import Storage
from utils import file_utils


async def documents_download(request: DownloadRequest) -> StringResult:
    document = document_repository.get_document(request.documentId)
    if document is None:
        raise HTTPException(status_code=400, detail="未找到文档")

    storage = Storage()
    file_path = file_utils.combine_url(
        document.dirPath, f"{document.uuid}{document.extName}"
    )
    file_url = storage.get_file_url(file_path)

    return StringResult(value=file_url)
