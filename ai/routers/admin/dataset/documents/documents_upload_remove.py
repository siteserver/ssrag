import os
from dto import BoolResult
from .__base import UploadRemoveRequest


async def documents_upload_remove(request: UploadRemoveRequest) -> BoolResult:
    task = request.task
    save_path = f"{task.dirPath}/{task.uuid}{task.extName}"
    os.remove(save_path)
    return BoolResult(value=True)
