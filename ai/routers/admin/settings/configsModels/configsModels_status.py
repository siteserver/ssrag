from celery_app import celery_app
from celery.result import AsyncResult
from .__base import StatusRequest, StatusResult


def configsModels_status(request: StatusRequest):
    task_result = AsyncResult(request.taskId, app=celery_app)

    return StatusResult(
        taskId=request.taskId,
        state=task_result.state,
        result=task_result.result if task_result.result else {},
        detail=str(task_result.info) if task_result.state == "FAILURE" else "",
    )
