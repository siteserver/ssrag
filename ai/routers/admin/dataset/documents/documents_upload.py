import os
import uuid
from fastapi import UploadFile
from utils import file_utils
from .__base import UploadResult
from dto import TaskDocumentProcess
from storages import Storage


async def documents_upload(
    siteId: int,
    channelId: int,
    contentId: int,
    file: UploadFile,
) -> UploadResult:
    contents = await file.read()
    original_name, ext_name = os.path.splitext(file.filename or "")
    file_name = str(uuid.uuid4())
    dir_path = file_utils.get_date_path()

    storage = Storage()
    file_path = file_utils.combine_url(dir_path, f"{file_name}{ext_name}")
    storage.save_stream(file_path, contents)

    task = TaskDocumentProcess(
        uuid=file_name,
        siteId=siteId,
        channelId=channelId,
        contentId=contentId,
        dirPath=dir_path,
        fileName=original_name,
        extName=ext_name,
    )

    return UploadResult(
        task=task,
    )
