from models import Site, FlowNode, FlowNodeSettings
from dto import RunVariable, RunProcess
from enums import NodeType
from repositories import flow_edge_repository, flow_node_repository
from fastapi.responses import StreamingResponse
from .app_manager_process_start import app_manager_process_start
from .app_manager_process_input import app_manager_process_input
from .app_manager_process_llm import app_manager_process_llm
from .app_manager_process_web_search import app_manager_process_web_search
from .app_manager_get_flow_node_settings import app_manager_get_flow_node_settings
from .app_manager_get_in_variables import app_manager_get_in_variables
from .app_manager_process_dataset import app_manager_process_dataset
from .app_manager_process_intent import app_manager_process_intent
from .app_manager_chat import app_manager_chat
from .app_manager_initialize import app_manager_initialize
from vectors import Vector
from models import ConfigValues


class AppManager:
    def __init__(self, site: Site):
        self.site = site
        self.vector = Vector()
        
    @classmethod
    def initialize(cls, config_values: ConfigValues) -> ConfigValues:
        return app_manager_initialize(config_values)

    def get_settings(self, flow_node: FlowNode) -> FlowNodeSettings:
        return app_manager_get_flow_node_settings(flow_node)

    def is_output(self, next_node: FlowNodeSettings) -> bool:
        is_output = next_node.nodeType == NodeType.OUTPUT
        if next_node.nodeType == NodeType.LLM:
            is_output = next_node.llmIsReply
        return is_output

    def get_in_variables(
        self,
        settings: FlowNodeSettings,
        inVariablesDict: dict[str, list[RunVariable]],
        outVariablesDict: dict[str, list[RunVariable]],
    ) -> list[RunVariable]:
        return app_manager_get_in_variables(settings, inVariablesDict, outVariablesDict)

    def get_next_node(self, settings: FlowNodeSettings) -> FlowNodeSettings | None:
        next_node = None

        # if node.nodeType == NodeType.INTENT:
        #     intention = next(
        #         (v.value for v in out_variables if v.name == "intention"), None
        #     )
        #     edges = self.app.get_right_edges(node.id)

        #     if not edges:
        #         return None

        #     node_intent = node.intentions
        #     if not node_intent:
        #         return None

        #     handle_id = self._get_handle_id(node_intent, intention)
        #     edge = next((e for e in edges if e.sourceHandle == handle_id), None)

        #     if not edge:
        #         return None

        #     next_node = self.app.get_node(edge.target)
        # else:
        next_node_uuid = flow_edge_repository.get_next_node_uuid(
            self.site.id, settings.id
        )
        next_node = flow_node_repository.get_by_uuid(self.site.id, next_node_uuid)
        if not next_node:
            return None

        return self.get_settings(next_node)

    def chat(self, message: str, thinking: bool, searching: bool) -> StreamingResponse:
        return app_manager_chat(self.site, self.vector, message, thinking, searching)

    def process(
        self,
        settings: FlowNodeSettings,
        inVariablesDict: dict[str, list[RunVariable]],
        outVariablesDict: dict[str, list[RunVariable]],
    ) -> StreamingResponse:
        inVariables = self.get_in_variables(settings, inVariablesDict, outVariablesDict)
        inVariablesDict[settings.id] = inVariables
        process = self.process_node(settings, inVariables)
        outVariables = process.outVariables
        outVariablesDict[settings.id] = outVariables
        next_node = self.get_next_node(settings)
        if next_node is not None:
            return self.process(next_node, inVariablesDict, outVariablesDict)
        if process.response is None:
            raise Exception("Response not found")
        return process.response

    def process_node(
        self, settings: FlowNodeSettings, inVariables: list[RunVariable]
    ) -> RunProcess:
        process = RunProcess(outVariables=[])

        if settings.nodeType == NodeType.START:
            process = app_manager_process_start(inVariables)
        elif settings.nodeType == NodeType.INPUT:
            process = app_manager_process_input(settings, inVariables)
        elif settings.nodeType == NodeType.WEBSEARCH:
            process = app_manager_process_web_search(settings, inVariables)
        elif settings.nodeType == NodeType.DATASET:
            process = app_manager_process_dataset(self.vector, settings, inVariables)
        elif settings.nodeType == NodeType.LLM:
            process = app_manager_process_llm(settings, inVariables)
        elif settings.nodeType == NodeType.INTENT:
            process = app_manager_process_intent(settings, inVariables)

        return process
