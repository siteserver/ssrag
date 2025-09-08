from .__base import GetResult
from repositories import celery_task_repository, document_repository
from enums import TaskStatus
from celery_app import celery_app
from celery.result import AsyncResult


async def status_get(siteId: int) -> GetResult:
    pending_count = celery_task_repository.get_pending_count(siteId)
    document_count = document_repository.get_total_count_by_site_id(siteId)
    celery_task_repository.delete_10_minutes_ago(siteId)
    tasks = celery_task_repository.get_pending_tasks(siteId, 10)
    for task in tasks:
        taskStatus = task.taskStatus
        taskResult = task.taskResult
        task_result = AsyncResult(task.taskId, app=celery_app)
        if task_result.failed():
            taskStatus = TaskStatus.FAILURE
            taskResult = str(task_result.result)
        else:
            taskStatus = task_result.state
            taskResult = str(task_result.result)

        if taskStatus != task.taskStatus or taskResult != task.taskResult:
            celery_task_repository.update_status(
                siteId=siteId,
                taskId=task.taskId,
                taskStatus=taskStatus,
                taskResult=taskResult,
            )
            task.taskStatus = taskStatus
            task.taskResult = taskResult

    return GetResult(
        tasks=tasks, pendingCount=pending_count, documentCount=document_count
    )
