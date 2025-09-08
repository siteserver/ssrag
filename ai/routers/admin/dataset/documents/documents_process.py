from celery_app import task_document_process
from .__base import ProcessRequest, ProcessResult


def documents_process(request: ProcessRequest) -> ProcessResult:
    task_ids = []
    for task in request.tasks:
        task = task_document_process.delay(
            uuid=task.uuid,
            siteId=task.siteId,
            channelId=task.channelId,
            contentId=task.contentId,
            dirPath=task.dirPath,
            fileName=task.fileName,
            extName=task.extName,
        )
        task_ids.append(task.id)
    return ProcessResult(taskIds=task_ids)
