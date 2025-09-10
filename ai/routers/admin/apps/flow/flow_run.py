from fastapi import HTTPException
from repositories import flow_node_repository, site_repository
from services import AppManager
from .__base import RunRequest, RunResult
from enums import NodeType


async def flow_run(request: RunRequest) -> RunResult:
    site = site_repository.get(request.siteId)
    if not site:
        raise HTTPException(status_code=404, detail="Site not found")

    app_manager = AppManager(site)
    node = flow_node_repository.get_by_uuid(request.siteId, request.nodeId)
    if not node:
        raise HTTPException(status_code=404, detail="Node not found")

    settings = app_manager.get_settings(node)
    if settings.nodeType == NodeType.LLM:
        settings.llmIsReply = False

    try:
        result = app_manager.process_node(settings, False, request.inVariables)
    except Exception as e:
        return RunResult(success=False, errorMessage=str(e))

    return RunResult(success=True, outVariables=result.outVariables)
