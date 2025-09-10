from fastapi import HTTPException
from repositories import flow_node_repository, site_repository
from services import AppManager
from .__base import RunNodeRequest, RunNodeResult


async def flow_run_node(request: RunNodeRequest) -> RunNodeResult:
    site = site_repository.get(request.siteId)
    if not site:
        raise HTTPException(status_code=404, detail="Site not found")

    app_manager = AppManager(site)
    node = flow_node_repository.get_by_uuid(request.siteId, request.nodeId)
    if not node:
        raise HTTPException(status_code=404, detail="Node not found")

    settings = app_manager.get_settings(node)
    inVariables = app_manager.get_in_variables(
        settings, request.inVariablesDict, request.outVariablesDict
    )

    nextNodeId = ""
    isOutput = False
    try:
        process = app_manager.process_node(settings, False, inVariables)
        outVariables = process.outVariables
        nextNode = app_manager.get_next_node(settings)
        if nextNode is None:
            raise HTTPException(status_code=404, detail="Next node not found")
        nextNodeId = nextNode.id
        isOutput = app_manager.is_output(nextNode)
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

    return RunNodeResult(
        isOutput=isOutput, nextNodeId=nextNodeId, outVariables=outVariables
    )
