import os
import uuid
from fastapi import UploadFile
from utils import file_utils
from dto import StringResult
from storages import Storage


async def publish_upload(
    file: UploadFile,
) -> StringResult:
    contents = await file.read()
    filename = file.filename or ""
    file_name, ext_name = os.path.splitext(filename)
    file_name = str(uuid.uuid4())

    storage = Storage()
    file_path = file_utils.combine_url(
        file_utils.get_date_path(), f"{file_name}{ext_name}"
    )
    storage.save_stream(file_path, contents, True)
    file_url = storage.get_file_url(file_path, False)

    return StringResult(value=file_url)
