from models.flow_node import FlowNode
from enums import SearchType
from utils.translate_utils import get_value, to_enum


class FlowNodeDataset:
    def __init__(self, node: FlowNode):
        self.datasetSearchType: SearchType
        self.datasetMaxCount: int
        self.datasetMinScore: float

        if node:
            self.load_dict(node.to_dict())

    def load_dict(self, node: dict):
        self.datasetSearchType = to_enum(
            get_value(node, "datasetSearchType", SearchType.SEMANTIC),
            SearchType,
            SearchType.SEMANTIC,
        )
        self.datasetMaxCount = get_value(node, "datasetMaxCount", 5)
        self.datasetMinScore = get_value(node, "datasetMinScore", 0.5)
