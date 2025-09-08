from repositories import (
    document_repository,
    content_repository,
    site_repository,
    channel_repository,
    segment_repository,
)
from dto import TaskContentProcess, TaskDocumentProcess
from utils import file_utils
from vectors import Vector
from storages import Storage
import uuid
from .document_process import document_process
from services import dataset_manager


def content_process(
    celery_app,
    task: TaskContentProcess,
):
    site = site_repository.get(task.siteId)
    if site is None:
        return

    channel = channel_repository.get(task.siteId, task.channelId)
    if channel is None or channel.knowledge == False:
        return

    content = content_repository.get(
        site.tableName, task.siteId, task.channelId, task.contentId
    )
    if content is None:
        return

    dataset_manager.remove_content(task.siteId, task.channelId, task.contentId)

    if content.body is None or len(content.body) == 0:
        if content.knowledge == True:
            content_repository.update_knowledge(
                site.tableName,
                task.siteId,
                task.channelId,
                task.contentId,
                False,
            )
        return

    file_uuid = str(uuid.uuid4())
    ext_name = ".html"

    storage = Storage()
    dir_path = file_utils.get_date_path()
    file_path = file_utils.combine_url(dir_path, f"{file_uuid}{ext_name}")
    storage.save_text(
        file_path,
        f"""
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>{content.title}</title>
</head>
<body>
{content.body}
</body>
</html>
        """,
    )

    task_document = TaskDocumentProcess(
        uuid=file_uuid,
        siteId=task.siteId,
        channelId=task.channelId,
        contentId=task.contentId,
        dirPath=dir_path,
        fileName=content.title,
        extName=ext_name,
    )

    return document_process(
        celery_app,
        task_document,
    )
