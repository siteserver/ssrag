from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Text
from utils import string_utils
from configs import table_names
from datetime import datetime
from enums import OutputFormat, NodeType, SearchType
from pydantic import BaseModel
from utils.translate_utils import get_value, to_enum


class Measured(BaseModel):
    width: int
    height: int


class Position(BaseModel):
    x: float
    y: float


class FlowNode(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_FLOW_NODE  # type: ignore
    id: int = Field(
        sa_column=Column("Id", Integer, primary_key=True),
        default=None,
    )
    uuid: str = Field(
        sa_column=Column("Uuid", String(50)),
        default=None,
    )
    createdDate: datetime = Field(
        sa_column=Column("CreatedDate", DateTime, default=datetime.now),
        default=None,
    )
    lastModifiedDate: datetime = Field(
        sa_column=Column("LastModifiedDate", DateTime, default=datetime.now),
        default=None,
    )
    siteId: int = Field(
        sa_column=Column("SiteId", Integer),
        default=None,
    )
    nodeType: str = Field(
        sa_column=Column("NodeType", String(500)),
        default=None,
    )
    title: str = Field(
        sa_column=Column("Title", String(500)),
        default=None,
    )
    description: str = Field(
        sa_column=Column("Description", String(500)),
        default=None,
    )
    extendValues: str = Field(
        sa_column=Column("ExtendValues", Text),
        default=None,
    )

    def to_dict(self) -> dict:
        return string_utils.to_json_object(self.extendValues)


class FlowNodeSettings(BaseModel):
    id: str = ""
    siteId: int = 0
    nodeId: int = 0
    nodeType: NodeType = NodeType.INPUT
    title: str = ""
    description: str = ""

    measured: Measured = Measured(width=100, height=100)
    position: Position = Position(x=500, y=200)

    # extendValues
    height: int = 100
    width: int = 100
    positionX: int = 500
    positionY: int = 200
    isFixed: bool = False

    providerModelId: str = ""
    modelTemperature: float = 0.1
    modelTopP: float = 0.7
    modelMaxResponseLength: int = 1024
    isIgnoreExceptions: bool = False
    chatHistoryEnabled: bool = False
    chatHistoryCount: int = 10

    # Dataset
    datasetSearchType: SearchType = SearchType.SEMANTIC
    datasetMaxCount: int = 5
    datasetMinScore: float = 0.5

    # LLM
    llmSystemPrompt: str = ""
    llmUserPrompt: str = ""
    llmIsReply: bool = False
    llmOutputFormat: OutputFormat = OutputFormat.MARKDOWN

    # WebSearch
    webSearchApiKey: str = ""
    webSearchFreshness: str = "noLimit"
    webSearchSummary: bool = False
    webSearchInclude: str = ""
    webSearchExclude: str = ""
    webSearchCount: int = 10

    # Intent
    intentPrompt: str = ""
    intentions: list[str] = []

    @classmethod
    def create(cls, flow_node: FlowNode) -> "FlowNodeSettings":
        data = flow_node.to_dict()
        return cls(
            id=flow_node.uuid,
            siteId=flow_node.siteId,
            nodeId=flow_node.id,
            nodeType=to_enum(flow_node.nodeType, NodeType, NodeType.INPUT),
            title=flow_node.title,
            description=flow_node.description,
            height=get_value(data, "height", 0),
            width=get_value(data, "width", 0),
            positionX=get_value(data, "positionX", 0),
            positionY=get_value(data, "positionY", 0),
            isFixed=get_value(data, "isFixed", False),
            providerModelId=get_value(data, "providerModelId", ""),
            modelTemperature=get_value(data, "modelTemperature", 0.1),
            modelTopP=get_value(data, "modelTopP", 0.7),
            modelMaxResponseLength=get_value(data, "modelMaxResponseLength", 1024),
            isIgnoreExceptions=get_value(data, "isIgnoreExceptions", False),
            chatHistoryEnabled=get_value(data, "chatHistoryEnabled", False),
            chatHistoryCount=get_value(data, "chatHistoryCount", 10),
            # Dataset
            datasetSearchType=to_enum(
                get_value(data, "datasetSearchType", ""),
                SearchType,
                SearchType.SEMANTIC,
            ),
            datasetMaxCount=get_value(data, "datasetMaxCount", 5),
            datasetMinScore=get_value(data, "datasetMinScore", 0.5),
            # LLM
            llmSystemPrompt=get_value(data, "llmSystemPrompt", ""),
            llmUserPrompt=get_value(data, "llmUserPrompt", ""),
            llmIsReply=get_value(data, "llmIsReply", False),
            llmOutputFormat=to_enum(
                get_value(data, "llmOutputFormat", ""),
                OutputFormat,
                OutputFormat.MARKDOWN,
            ),
            # WebSearch
            webSearchApiKey=get_value(data, "webSearchApiKey", ""),
            webSearchFreshness=get_value(data, "webSearchFreshness", "noLimit"),
            webSearchSummary=get_value(data, "webSearchSummary", False),
            webSearchInclude=get_value(data, "webSearchInclude", ""),
            webSearchExclude=get_value(data, "webSearchExclude", ""),
            webSearchCount=get_value(data, "webSearchCount", 10),
            # Intent
            intentPrompt=get_value(data, "intentPrompt", ""),
            intentions=get_value(data, "intentions", []),
        )

    def to_extend_values(self) -> dict:
        return {
            "height": self.height,
            "width": self.width,
            "positionX": self.positionX,
            "positionY": self.positionY,
            "isFixed": self.isFixed,
            "providerModelId": self.providerModelId,
            "modelTemperature": self.modelTemperature,
            "modelTopP": self.modelTopP,
            "modelMaxResponseLength": self.modelMaxResponseLength,
            "isIgnoreExceptions": self.isIgnoreExceptions,
            "chatHistoryEnabled": self.chatHistoryEnabled,
            "chatHistoryCount": self.chatHistoryCount,
            # Dataset
            "datasetSearchType": self.datasetSearchType.value,
            "datasetMaxCount": self.datasetMaxCount,
            "datasetMinScore": self.datasetMinScore,
            # LLM
            "llmSystemPrompt": self.llmSystemPrompt,
            "llmUserPrompt": self.llmUserPrompt,
            "llmIsReply": self.llmIsReply,
            "llmOutputFormat": self.llmOutputFormat.value,
            # WebSearch
            "webSearchApiKey": self.webSearchApiKey,
            "webSearchFreshness": self.webSearchFreshness,
            "webSearchSummary": self.webSearchSummary,
            "webSearchInclude": self.webSearchInclude,
            "webSearchExclude": self.webSearchExclude,
            "webSearchCount": self.webSearchCount,
            # Intent
            "intentPrompt": self.intentPrompt,
            "intentions": self.intentions,
        }
