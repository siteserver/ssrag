from pydantic import BaseModel
from dto import RunVariable
from models import FlowNodeSettings, FlowEdge, FlowVariable, SiteSummary
from enums import VariableType
from dto import Model


class GetResult(BaseModel):
    siteName: str
    flowNodesSettings: dict[str, FlowNodeSettings]
    flowNodesInVariables: dict[str, list[FlowVariable]]
    flowNodesOutVariables: dict[str, list[FlowVariable]]
    nodes: list[FlowNodeSettings]
    edges: list[FlowEdge]
    models: list[Model]
    defaultModel: Model | None


class OptimizeRequest(BaseModel):
    items: list[str]


class OptimizeResult(BaseModel):
    items: list[str]


class EdgeSubmit(BaseModel):
    id: str
    source: str
    sourceHandle: str
    target: str
    targetHandle: str


class VariableSubmit(BaseModel):
    id: int | None = None
    nodeId: str | None = None
    type: VariableType | None = None
    isDisabled: bool | None = None
    name: str | None = None
    dataType: str | None = None
    isReference: bool | None = None
    referenceNodeId: str | None = None
    referenceName: str | None = None
    value: str | None = None


class SubmitRequest(BaseModel):
    siteId: int
    nodes: list[FlowNodeSettings]
    edges: list[EdgeSubmit]
    variables: list[VariableSubmit]


class SubmitResult(BaseModel):
    nodes: list[FlowNodeSettings]
    edges: list[FlowEdge]


class RunRequest(BaseModel):
    siteId: int
    nodeId: str
    inVariables: list[RunVariable]


class RunResult(BaseModel):
    success: bool
    errorMessage: str | None = None
    outVariables: list[RunVariable] = []


class RunNodeRequest(BaseModel):
    siteId: int
    nodeId: str
    inVariablesDict: dict[str, list[RunVariable]]
    outVariablesDict: dict[str, list[RunVariable]]


class RunNodeResult(BaseModel):
    isOutput: bool
    nextNodeId: str
    outVariables: list[RunVariable]


class RunOutputRequest(BaseModel):
    siteId: int
    nodeId: str
    inVariablesDict: dict[str, list[RunVariable]]
    outVariablesDict: dict[str, list[RunVariable]]


class DatasetRequest(BaseModel):
    siteId: int
    nodeId: str


class DatasetResult(BaseModel):
    datasetSites: list[SiteSummary]
