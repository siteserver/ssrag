from fastapi import APIRouter
from .flow_get import flow_get
from .flow_submit import flow_submit
from .__base import (
    SubmitRequest,
    GetResult,
    OptimizeRequest,
    OptimizeResult,
    SubmitResult,
    RunRequest,
    RunResult,
    RunNodeRequest,
    RunNodeResult,
    RunOutputRequest,
    DatasetRequest,
    DatasetResult,
)
from configs import router_prefix
from .flow_optimize import flow_optimize
from .flow_run import flow_run
from .flow_run_node import flow_run_node
from .flow_run_output import flow_run_output
from .flow_dataset import flow_dataset

router = APIRouter(prefix=router_prefix.ADMIN_APPS_FLOW, tags=["apps/flow"])


@router.get("")
async def get(siteId: int) -> GetResult:
    return await flow_get(siteId)


@router.post("")
async def submit(request: SubmitRequest) -> SubmitResult:
    return await flow_submit(request)


@router.post("/actions/optimize")
async def optimize(request: OptimizeRequest) -> OptimizeResult:
    return await flow_optimize(request)


@router.post("/actions/run")
async def run(request: RunRequest) -> RunResult:
    return await flow_run(request)


@router.post("/actions/runNode")
async def run_node(request: RunNodeRequest) -> RunNodeResult:
    return await flow_run_node(request)


@router.post("/actions/runOutput")
async def run_output(request: RunOutputRequest):
    return await flow_run_output(request)


@router.post("/actions/dataset")
async def dataset(request: DatasetRequest) -> DatasetResult:
    return await flow_dataset(request)
