from celery_app import celery_app
from celery.result import AsyncResult
from .__base import StatusRequest, StatusResult
from enums import TaskStatus


def documents_status(request: StatusRequest):
    results = []
    for task_id in request.taskIds:
        task_result = AsyncResult(task_id, app=celery_app)
        if task_result.failed():
            error = task_result.result  # 获取抛出的异常对象
            traceback = task_result.traceback  # 完整堆栈跟踪
            results.append(
                {
                    "taskId": task_id,
                    "state": TaskStatus.FAILURE,
                    "result": str(error),
                    "detail": str(traceback),
                }
            )
        else:
            results.append(
                {
                    "taskId": task_id,
                    "state": task_result.state,
                    "result": task_result.result,
                }
            )

    return StatusResult(results=results)
