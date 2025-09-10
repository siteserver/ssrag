from fastapi import HTTPException
from fastapi.responses import StreamingResponse
from repositories import flow_node_repository, site_repository
from services import AppManager
from .__base import RunOutputRequest


async def flow_run_output(request: RunOutputRequest) -> StreamingResponse:
    site = site_repository.get(request.siteId)
    if not site:
        raise HTTPException(status_code=404, detail="Site not found")

    app_manager = AppManager(site)
    node = flow_node_repository.get_by_uuid(request.siteId, request.nodeId)
    if not node:
        raise HTTPException(status_code=404, detail="Node not found")

    settings = app_manager.get_settings(node)
    settings.llmIsReply = True

    inVariables = app_manager.get_in_variables(
        settings, request.inVariablesDict, request.outVariablesDict
    )

    result = app_manager.process_node(settings, False, inVariables)

    if result.response is None:
        raise HTTPException(status_code=404, detail="Response not found")

    return result.response
