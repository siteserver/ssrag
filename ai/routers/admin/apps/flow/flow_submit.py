from .__base import SubmitRequest, SubmitResult
from repositories import (
    flow_node_repository,
    flow_edge_repository,
    flow_variable_repository,
    site_repository,
)
from models import FlowEdge, FlowVariable
from enums import NodeType
from services import AppManager
from fastapi import HTTPException
import uuid


async def flow_submit(request: SubmitRequest) -> SubmitResult:
    site = site_repository.get(request.siteId)
    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    flow_node_repository.delete_all(request.siteId)
    flow_edge_repository.delete_all(request.siteId)
    flow_variable_repository.delete_all(request.siteId)

    app_manager = AppManager(site)
    for settings in request.nodes:
        settings.siteId = request.siteId
        settings.nodeType = NodeType(settings.nodeType)
        if settings.measured:
            settings.width = settings.measured.width
            settings.height = settings.measured.height
        if settings.position:
            settings.positionX = int(settings.position.x)
            settings.positionY = int(settings.position.y)
        flow_node_repository.insert(settings.id, settings)

    for submit_edge in request.edges:
        flow_edge_repository.insert(
            FlowEdge(
                uuid=str(uuid.uuid4()),
                siteId=request.siteId,
                source=submit_edge.source,
                sourceHandle=submit_edge.sourceHandle,
                target=submit_edge.target,
                targetHandle=submit_edge.targetHandle,
            )
        )

    for variable_submit in request.variables:
        variable = FlowVariable(
            uuid=str(uuid.uuid4()),
            siteId=request.siteId,
        )
        variable.nodeId = variable_submit.nodeId or ""
        variable.type = variable_submit.type or ""
        variable.name = variable_submit.name or ""
        variable.dataType = variable_submit.dataType or ""
        variable.isDisabled = variable_submit.isDisabled or False
        if variable_submit.isReference:
            variable.isReference = True
            variable.referenceNodeId = variable_submit.referenceNodeId or ""
            variable.referenceName = variable_submit.referenceName or ""
        else:
            variable.isReference = False
            variable.value = variable_submit.value or ""
        flow_variable_repository.insert(variable)

    flow_nodes = flow_node_repository.get_all(request.siteId)
    nodes = []
    for node in flow_nodes:
        nodes.append(app_manager.get_settings(node))
    flow_edges = flow_edge_repository.get_all(request.siteId)

    return SubmitResult(nodes=nodes, edges=flow_edges)
