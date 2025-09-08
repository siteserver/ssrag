from repositories import (
    celery_task_repository,
)
from celery_app import task_content_process


def dataset_manager_index_content(siteId: int, channelId: int, contentId: int):
    task = task_content_process.delay(
        siteId=siteId,
        channelId=channelId,
        contentId=contentId,
    )
    if task and task.id:
        celery_task_repository.insert(
            siteId=siteId,
            channelId=channelId,
            contentId=contentId,
            taskId=task.id,
        )
