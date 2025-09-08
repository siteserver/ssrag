from fastapi import APIRouter
from .__base import IndexRequest, RemoveRequest
from configs import router_prefix
from dto import BoolResult


router = APIRouter(prefix=router_prefix.ADMIN_DATASET_TASKS, tags=["dataset/tasks"])


@router.post("/actions/index")
async def index(request: IndexRequest) -> BoolResult:
    from .tasks_index import tasks_index

    return await tasks_index(request)


@router.post("/actions/remove")
async def remove(request: RemoveRequest) -> BoolResult:
    from .tasks_remove import tasks_remove

    return await tasks_remove(request)
