from fastapi import HTTPException
from repositories import site_repository, flow_node_repository, flow_variable_repository
from .__base import GetResult
from models import FlowNodeSettings
from enums import NodeType, VariableType, VariableDataType
from repositories import flow_edge_repository
from services import AppManager
from models import FlowVariable
from configs import constants
from enums import ModelType
from services import llm_manager
import uuid


async def flow_get(siteId: int) -> GetResult:
    site = site_repository.get(siteId)
    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    app_manager = AppManager(site)

    flow_nodes = flow_node_repository.get_all(siteId)
    if not any(flow_node.nodeType == NodeType.START for flow_node in flow_nodes):
        start_settings = FlowNodeSettings()
        start_settings.siteId = siteId
        start_settings.nodeType = NodeType.START
        start_settings.title = "流程开始"
        start_settings.positionX = 500
        start_settings.positionY = 200
        start_settings.isFixed = True
        flow_node_repository.insert(str(uuid.uuid4()), start_settings)
        flow_nodes = flow_node_repository.get_all(siteId)

    nodes_settings = {}
    nodes_in_variables = {}
    nodes_out_variables = {}
    nodes = []
    for flow_node in flow_nodes:
        settings = app_manager.get_settings(flow_node)
        inVariables = flow_variable_repository.get_variables(
            flow_node.siteId, settings.id, VariableType.INPUT
        )
        if inVariables is None or len(inVariables) == 0:
            inVariables = [
                FlowVariable(
                    uuid=str(uuid.uuid4()),
                    siteId=siteId,
                    nodeId=settings.id,
                    type=VariableType.INPUT,
                    name=constants.DEFAULT_INPUT_NAME,
                    dataType=VariableDataType.STRING.value,
                    description="",
                )
            ]
        outVariables = flow_variable_repository.get_variables(
            flow_node.siteId, settings.id, VariableType.OUTPUT
        )
        if outVariables is None or len(outVariables) == 0:
            outVariables = [
                FlowVariable(
                    uuid=str(uuid.uuid4()),
                    siteId=siteId,
                    nodeId=settings.id,
                    type=VariableType.OUTPUT,
                    name=constants.DEFAULT_OUTPUT_NAME,
                    dataType=VariableDataType.STRING.value,
                    description="",
                )
            ]
        nodes_settings[settings.id] = settings
        nodes_in_variables[settings.id] = inVariables
        nodes_out_variables[settings.id] = outVariables
        nodes.append(settings)

    edges = flow_edge_repository.get_all(siteId)
    models, defaultModel = llm_manager.get_models(ModelType.LLM)

    return GetResult(
        siteName=site.siteName,
        flowNodesSettings=nodes_settings,
        flowNodesInVariables=nodes_in_variables,
        flowNodesOutVariables=nodes_out_variables,
        nodes=nodes,
        edges=edges,
        models=models,
        defaultModel=defaultModel,
    )
